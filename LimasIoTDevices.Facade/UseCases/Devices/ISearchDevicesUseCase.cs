using LimasIoTDevices.Facade.Dtos;

namespace LimasIoTDevices.Facade.UseCases.Devices;

public interface ISearchDevicesUseCase : IUseCaseBase
{
    Task<List<GetDeviceResponse>> Execute(string? searchTerm);
}
