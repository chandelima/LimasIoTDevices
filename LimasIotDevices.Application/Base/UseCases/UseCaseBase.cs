using LimasIoTDevices.Shared.DTOs;
using LimasIoTDevices.Shared.Interfaces;

namespace LimasIotDevices.Application.Base.UseCases;

internal abstract class UseCaseBase : IHasMessage
{
    public List<MessageResponse> Messages { get; set; } = [];
}
