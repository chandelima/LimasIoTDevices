using LimasIoTDevices.Shared.Enumerators;

namespace LimasIoTDevices.Shared.DTOs;

public class MessageResponse
{
    public MessageResponse() { }

    public MessageResponse(EnumResponseMessageType type, string message)
    {
        Type = type;
        Message = message;
    }

    public EnumResponseMessageType Type { get; set; }
    public string Message { get; set; } = null!;
}
