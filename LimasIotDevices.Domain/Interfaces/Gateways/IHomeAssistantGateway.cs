namespace LimasIotDevices.Domain.Interfaces.Gateways;

public interface IHomeAssistantGateway
{
    Task<bool> CallService(string entityId, string service, Dictionary<string, object?>? serviceData);
    Task<bool> DeviceExists(string entityId);
    Task<IEnumerable<(string EntityId, string State, TimeSpan Duration)>> GetAllDevicesStatesWithDuration();
    Task<(string EntityId, string State, TimeSpan Duration)?> GetDeviceStateWithDuration(string entityId);
}