using LimasIoTDevices.Contracts.Dtos;
using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IUpdateDevicesUseCase : IHasMessage
{
    Task<bool> Execute(CreateUpdateDeviceRequest request);
}