using LimasIoTDevices.Contracts.Dtos;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface IGetDevicesStateUseCase : IUseCaseBase
{
    Task<GetDeviceStateResponse?> Execute(string deviceKey, string? attributeKey);
}
