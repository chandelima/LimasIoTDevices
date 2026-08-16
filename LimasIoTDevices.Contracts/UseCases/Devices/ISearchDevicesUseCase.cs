using LimasIoTDevices.Contracts.Dtos;

namespace LimasIoTDevices.Contracts.UseCases.Devices;

public interface ISearchDevicesUseCase : IUseCaseBase
{
    Task<List<GetDeviceResponse>> Execute(string? searchTerm);
}
