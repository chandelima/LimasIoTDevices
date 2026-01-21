using LimasIoTDevices.Shared.Attributes;

namespace LimasIoTDevices.Facade.Enumerators;

public enum EnumEventType
{
    [StringValue("connection-established")]
    ConnectionEstablished,

    [StringValue("device-state-changed")]
    DeviceStateChanged
}
