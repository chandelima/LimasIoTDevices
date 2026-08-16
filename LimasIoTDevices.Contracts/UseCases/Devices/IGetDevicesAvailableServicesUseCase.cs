using LimasIoTDevices.Contracts.Dtos;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IGetDevicesAvailableServicesUseCase : IUseCaseBase
{
    Task<List<GetDeviceAvailableServicesResponse>> Execute();
}
