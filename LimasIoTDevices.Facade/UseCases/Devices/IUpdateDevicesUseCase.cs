using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IUpdateDevicesUseCase : IHasMessage
{
    Task<bool> Execute(CreateUpdateDeviceRequest request);
}