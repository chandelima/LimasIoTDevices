namespace LimasIoTDevices.Facade.Dtos;

public record CreateUpdateDeviceRequest(string? Key, string? Name, string? Description, List<CreateUpdateDeviceAttributeRequest>? Attributes);
public record GetDeviceResponse(string Key, string Name, string? Description, List<GetDeviceAttributeResponse> Attributes);
public record GetDeviceStateResponse(string EntityKey, List<GetDeviceAttributeStateResponse> Attributes);
public record GetDeviceAttributeStateResponse(string AttributeKey, object State, double CurrentStateDuration);

public record CreateUpdateDeviceAttributeRequest(string? Key, string? Name, string? Description, string[]? Entities);
public record GetDeviceAttributeResponse(string Key, string Name, string? Description, string[] Entities);

