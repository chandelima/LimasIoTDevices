using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Application.Constants;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LimasIoTDevices.Facade.UseCases.Devices;

internal class GetDevicesStateUseCase(
    LimasIotDevicesDbContext dbContext,
    IHomeAssistantGateway homeAssistantGateway) : UseCaseBase, IGetDevicesStateUseCase
{
    public async Task<GetDeviceStateResponse?> Execute(string deviceKey, string? attributeKey)
    {
        attributeKey ??= ApplicationConstants.MAIN_KEY_NAME;

        var databaseDevice = await dbContext.Attributes
            .FirstOrDefaultAsync(x => x.Key == attributeKey
                                   && x.DeviceKey == deviceKey);

        if (databaseDevice is null) return null;

        var haStates = await homeAssistantGateway.GetAllDevicesStatesWithDuration();
        var entitiesStates = haStates
            .Where(x => databaseDevice.Entities.Contains(x.EntityId))
            .OrderBy(x => x.CurrentStateDuration.TotalSeconds)
            .ToList();

        object state = "undefined";
        double currentStateDuration = 0;

        foreach (var entityState in entitiesStates)
        {
            if (entityState.State != "undefined")
            {
                state = entityState.State;
                currentStateDuration = entityState.CurrentStateDuration.TotalSeconds;

                break;
            }
        }

        var attributeResponse = new GetDeviceAttributeStateResponse(databaseDevice.Key, state, currentStateDuration);
        var deviceResponse = new GetDeviceStateResponse(deviceKey, [attributeResponse]);

        return deviceResponse;
    }
}
