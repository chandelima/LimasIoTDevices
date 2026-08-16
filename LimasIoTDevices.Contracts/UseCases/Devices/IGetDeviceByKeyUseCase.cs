using LimasIoTDevices.Contracts.Dtos;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IGetDeviceByKeyUseCase : IUseCaseBase
{
    Task<GetDeviceResponse?> Execute(string key);
}
