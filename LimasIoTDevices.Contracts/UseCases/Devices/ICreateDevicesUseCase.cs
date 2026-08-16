using LimasIoTDevices.Contracts.Dtos;
using LimasIoTDevices.Shared.Interfaces;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface ICreateDevicesUseCase : IHasMessage
{
    Task<bool> Execute(CreateUpdateDeviceRequest request);
}
