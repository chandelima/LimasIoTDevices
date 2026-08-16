using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IRemoveDevicesUseCase : IHasMessage
{
    Task<bool> Execute(string key);
}
