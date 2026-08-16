using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface ICreateDevicesUseCase : IHasMessage
{
    Task<bool> Execute(CreateUpdateDeviceRequest request);
}
