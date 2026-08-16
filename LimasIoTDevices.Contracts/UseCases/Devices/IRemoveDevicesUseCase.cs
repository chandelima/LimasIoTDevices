using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IRemoveDevicesUseCase : IHasMessage
{
    Task<bool> Execute(string key);
}
