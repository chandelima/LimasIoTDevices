using LimasIoTDevices.Shared.Services;

namespace LimasIotDevices.Infrastructure.Handlers;

internal class HomeAssistantTokenDelegatingHandler : DelegatingHandler
{
    private readonly string _token;

    public HomeAssistantTokenDelegatingHandler(IGetConfigurationService getConfigurationService)
    {
        _token = getConfigurationService.Execute<string>("HomeAssistantData:Token");
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        return await base.SendAsync(request, cancellationToken);
    }
}
