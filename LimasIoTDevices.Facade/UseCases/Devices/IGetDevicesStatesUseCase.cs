using LimasIoTDevices.Facade.Dtos;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IGetDevicesStatesUseCase : IUseCaseBase
{
    Task<List<GetDeviceStateResponse>> Execute();
}
