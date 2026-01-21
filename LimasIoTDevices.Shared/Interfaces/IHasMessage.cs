using LimasIoTDevices.Shared.DTOs;

namespace LimasIoTDevices.Shared.Interfaces;

public interface IHasMessage
{
    public List<MessageResponse> Messages { get; set; }
}
