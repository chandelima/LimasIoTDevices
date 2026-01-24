using LimasIoTDevices.Facade.Dtos;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IGetDevicesAvailableServicesUseCase : IUseCaseBase
{
    Task<List<GetDeviceAvailableServicesResponse>> Execute();
}
