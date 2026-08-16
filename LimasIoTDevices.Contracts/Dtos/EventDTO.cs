using LimasIoTDevices.Contracts.Enumerators;

namespace LimasIoTDevices.Contracts.Dtos;

public record EventResponse(EnumEventType Event, object Data);

public record DeviceStateChangedResponse(string DeviceKey, string AttributeKey, string NewState);