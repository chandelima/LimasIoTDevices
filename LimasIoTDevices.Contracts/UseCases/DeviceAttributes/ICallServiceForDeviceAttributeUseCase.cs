namespace LimasIoTDevices.Contracts.UseCases.DeviceAttributes;

public interface ICallServiceForDeviceAttributeUseCase
{
    Task<bool> Execute(string device, string? attributeKey, string service, Dictionary<string, object?>? data = null);
}
