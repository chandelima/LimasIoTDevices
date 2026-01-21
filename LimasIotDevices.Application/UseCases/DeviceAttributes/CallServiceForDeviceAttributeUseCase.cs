using LimasIotDevices.Application.Constants;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.UseCases.DeviceAttributes;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.UseCases.DeviceAttributes;

internal class CallServiceForDeviceAttributeUseCase(
    LimasIotDevicesDbContext _dbContext,
    IHomeAssistantGateway _homeAssistantGateway) : ICallServiceForDeviceAttributeUseCase
{
    public async Task<bool> Execute(string device, string? attributeKey, string service, Dictionary<string, object?>? data = null)
    {
        attributeKey ??= ApplicationConstants.MAIN_KEY_NAME;

        var databaseAttribute = await _dbContext.Attributes
            .FirstOrDefaultAsync(x => x.DeviceKey.ToLower() == device.ToLower().Trim()
                                   && x.Key.ToLower() == attributeKey.Trim().ToLower());

        if (databaseAttribute is null)
        {
            return false;
        }

        foreach (var entity in databaseAttribute.Entities)
        {
            var result = await _homeAssistantGateway.CallService(entity, service, data);

            if (result)
            {
                return true;
            }
        }

        return false;
    }
}
