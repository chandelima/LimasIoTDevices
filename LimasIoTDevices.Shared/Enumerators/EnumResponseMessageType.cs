using LimasIoTDevices.Shared.Attributes;

namespace LimasIoTDevices.Shared.Enumerators;

public enum EnumResponseMessageType
{

    [StringValue("error")]
    Error,

    [StringValue("information")]
    Information,

    [StringValue("warning")]
    Warning,

    [StringValue("success")]
    Success
}