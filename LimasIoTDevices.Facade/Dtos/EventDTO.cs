using LimasIoTDevices.Facade.Enumerators;

namespace LimasIoTDevices.Facade.Dtos;

public record EventResponse(EnumEventType Event, object Data);

public record DeviceStateChangedResponse(string DeviceKey, string AttributeKey, string NewStatus);