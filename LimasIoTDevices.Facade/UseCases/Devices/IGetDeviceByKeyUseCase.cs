using LimasIoTDevices.Facade.Dtos;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IGetDeviceByKeyUseCase : IUseCaseBase
{
    Task<GetDeviceResponse?> Execute(string key);
}
