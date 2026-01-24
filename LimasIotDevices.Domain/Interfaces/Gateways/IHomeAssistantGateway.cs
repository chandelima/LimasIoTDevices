namespace LimasIotDevices.Domain.Interfaces.Gateways;

public interface IHomeAssistantGateway
{
    Task<bool> CallService(string entityId, string service, Dictionary<string, object?>? serviceData);
    Task<bool> DeviceExists(string entityId);
    Task<List<(string EntityId, string State, TimeSpan CurrentStateDuration)>> GetAllDevicesStatesWithDuration();
    Task<(string EntityId, string State, TimeSpan CurrentStateDuration)?> GetDeviceStateWithDuration(string entityId);
    Task<Dictionary<string, HashSet<string>>> GetAvailableServices();
}