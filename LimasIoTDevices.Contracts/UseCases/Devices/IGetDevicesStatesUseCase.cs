using LimasIoTDevices.Contracts.Dtos;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IGetDevicesStatesUseCase : IUseCaseBase
{
    Task<List<GetDeviceStateResponse>> Execute();
}
