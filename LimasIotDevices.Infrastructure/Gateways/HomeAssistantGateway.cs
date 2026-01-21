using LimasIotDevices.Domain.Interfaces.Gateways;
using System.Text;
using System.Text.Json;

namespace LimasIotDevices.Infrastructure.Gateways;

internal class HomeAssistantGateway : IHomeAssistantGateway
{
    private readonly HttpClient _httpClient;

    public HomeAssistantGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> DeviceExists(string entityId)
    {
        var response = await _httpClient.GetAsync($"/api/states/{entityId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<(string EntityId, string State, TimeSpan Duration)>> GetAllDevicesStatesWithDuration()
    {
        var response = await _httpClient.GetAsync("/api/states");

        if (!response.IsSuccessStatusCode)
            return Enumerable.Empty<(string, string, TimeSpan)>();

        var json = await response.Content.ReadAsStringAsync();

        var results = new List<(string EntityId, string State, TimeSpan Duration)>();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Array)
            return Enumerable.Empty<(string, string, TimeSpan)>();

        foreach (var entity in root.EnumerateArray())
        {
            if (!entity.TryGetProperty("entity_id", out var entityIdProp) ||
                !entity.TryGetProperty("state", out var stateProp) ||
                !entity.TryGetProperty("last_changed", out var lastChangedProp))
            {
                continue;
            }

            var entityId = entityIdProp.GetString();
            var state = stateProp.GetString();
            var lastChangedStr = lastChangedProp.GetString();

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(lastChangedStr))
                continue;

            if (!DateTime.TryParse(lastChangedStr, out var lastChangedUtc))
                continue;

            var duration = DateTime.UtcNow - lastChangedUtc.ToUniversalTime();

            results.Add((entityId, state, duration));
        }

        return results;
    }


    public async Task<(string EntityId, string State, TimeSpan Duration)?> GetDeviceStateWithDuration(string entityId)
    {
        var response = await _httpClient.GetAsync($"/api/states/{entityId}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("state", out var stateProp) ||
            !root.TryGetProperty("last_changed", out var lastChangedProp))
        {
            return null;
        }

        var state = stateProp.GetString();
        var lastChangedStr = lastChangedProp.GetString();

        if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(lastChangedStr))
        {
            return null;
        }

        if (!DateTime.TryParse(lastChangedStr, out var lastChangedUtc))
        {
            return null;
        }

        var duration = DateTime.UtcNow - lastChangedUtc.ToUniversalTime();

        return (entityId, state, duration);
    }

    public async Task<bool> CallService(
        string entityId,
        string service,
        Dictionary<string, object?>? serviceData)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            return false;
        }

        var dotIndex = entityId.IndexOf('.');
        if (dotIndex <= 0)
        {
            return false;
        }

        var domain = entityId[..dotIndex];

        Dictionary<string, object?> payload;

        if (serviceData == null)
        {
            payload = new Dictionary<string, object?>
            {
                ["entity_id"] = entityId
            };
        }
        else
        {
            payload = new Dictionary<string, object?>
            {
                ["entity_id"] = entityId
            };

            foreach (var prop in serviceData.GetType().GetProperties())
            {
                payload[prop.Name] = prop.GetValue(serviceData);
            }
        }

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync(
                $"/api/services/{domain}/{service}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
