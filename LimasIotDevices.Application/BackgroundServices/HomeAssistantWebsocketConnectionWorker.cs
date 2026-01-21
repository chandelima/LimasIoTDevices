using LimasIotDevices.Application.Services.Device;
using LimasIoTDevices.Facade.Services;
using LimasIoTDevices.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LimasIotDevices.Application.BackgroundServices;

internal class HomeAssistantWebsocketConnectionWorker : IHostedService
{
    private readonly Uri _uri;
    private readonly string _token;
    private readonly ILogger<HomeAssistantWebsocketConnectionWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private ClientWebSocket? _socket;
    private CancellationTokenSource? _cts;
    private Task? _listeningTask;

    public HomeAssistantWebsocketConnectionWorker(
        IGetConfigurationService getConfigurationService,
        ILogger<HomeAssistantWebsocketConnectionWorker> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        var haUrlBase = getConfigurationService.Execute<string>("HomeAssistantData:HostUrl");

        if (haUrlBase.EndsWith("/"))
        {
            haUrlBase = haUrlBase[..^1];
        }

        var isHttps = haUrlBase.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        haUrlBase = haUrlBase.Replace(isHttps ? "https://" : "http://", isHttps ? "wss://" : "ws://");
        haUrlBase += "/api/websocket";

        _uri = new Uri(haUrlBase);
        _token = getConfigurationService.Execute<string>("HomeAssistantData:Token");
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _listeningTask = Task.Run(() => ListenLoop(_cts.Token));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();

        if (_socket != null && _socket.State == WebSocketState.Open)
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client shutdown", cancellationToken);

        if (_listeningTask != null)
            await _listeningTask;
    }

    private async Task ListenLoop(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _socket?.Dispose();
                _socket = new ClientWebSocket();

                _logger.LogInformation("Conectando ao Home Assistant...");
                await _socket.ConnectAsync(_uri, cancellationToken);
                _logger.LogInformation("Conectado!");

                await PerformHandshakeAndSubscribe(cancellationToken);

                while (_socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(buffer, cancellationToken);
                    var response = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    JsonNode? root = JsonNode.Parse(response);
                    if (root?["type"]?.ToString() == "event" &&
                        root["event"]?["event_type"]?.ToString() == "state_changed")
                    {
                        var entityId = root["event"]?["data"]?["entity_id"]?.ToString();
                        var newState = root["event"]?["data"]?["new_state"]?["state"]?.ToString();

                        if (entityId != null && newState != null)
                        {
                            HandleStateChange(entityId, newState);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha na conexão WebSocket. Tentando reconectar em 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task PerformHandshakeAndSubscribe(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        // Aguarda auth_required
        await _socket!.ReceiveAsync(buffer, cancellationToken);

        // Envia token
        await Send(new
        {
            type = "auth",
            access_token = _token
        }, cancellationToken);

        // Aguarda auth_ok
        await _socket.ReceiveAsync(buffer, cancellationToken);

        // Subscrição aos eventos
        await Send(new
        {
            id = 1,
            type = "subscribe_events",
            @event_type = "state_changed"
        }, cancellationToken);
    }

    private async Task Send(object obj, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _socket!.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }

    private void HandleStateChange(string entityId, string newState)
    {
        if (string.IsNullOrWhiteSpace(entityId) || string.IsNullOrWhiteSpace(newState))
        {
            return;
        }
        
        _logger.LogInformation("Evento recebido: {EntityId} -> {NewState}", entityId, newState);
        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var sendDeviceEventService = scope.ServiceProvider.GetRequiredService<SendDeviceEventService>();
            await sendDeviceEventService.Execute(entityId, newState);
        });
    }
}
