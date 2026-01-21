using LimasIotDevices.Domain.Interfaces.Gateways;

namespace LimasIotDevices.Application.Services.HomeAssistant;

internal class GetExistentHomeAssistantDevicesService(IHomeAssistantGateway gateway)
{
    public async Task<List<(string EntityId, string State, TimeSpan Duration)>> Execute(IEnumerable<string> entities)
    {
        var haDevices = await gateway.GetAllDevicesStatesWithDuration();
        return haDevices
            .Where(x => entities.Contains(x.EntityId))
            .ToList();
    }
}
