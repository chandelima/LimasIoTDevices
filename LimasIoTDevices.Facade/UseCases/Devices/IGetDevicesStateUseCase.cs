using LimasIoTDevices.Facade.Dtos;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface IGetDevicesStateUseCase : IUseCaseBase
{
    Task<GetDeviceStateResponse?> Execute(string deviceKey, string? attributeKey);
}
