using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.UseCases.Devices;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.Services.Device;

internal class GetDevicesStatesUseCase(
    LimasIotDevicesDbContext dbContext,
    IHomeAssistantGateway homeAssistantGateway) : UseCaseBase, IGetDevicesStatesUseCase
{
    public async Task<List<GetDeviceStateResponse>> Execute()
    {
        var databaseDevicesTask = dbContext.Devices.Include(x => x.Attributes).ToListAsync();
        var haGatewayDeviceStatusTask = homeAssistantGateway.GetAllDevicesStatesWithDuration();

        await Task.WhenAll(databaseDevicesTask, haGatewayDeviceStatusTask);

        var databaseDevices = await databaseDevicesTask;
        var haGatewayDeviceStatus = await haGatewayDeviceStatusTask;

        var devicesResponseList = new List<GetDeviceStateResponse>();

        foreach (var device in databaseDevices)
        {
            var attributesResponseList = new List<GetDeviceAttributeStateResponse>();

            foreach (var attribute in device.Attributes)
            {
                var entitiesStates = haGatewayDeviceStatus
                    .Where(x => attribute.Entities.Contains(x.EntityId))
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

                var attributeResponse = new GetDeviceAttributeStateResponse(attribute.Key, state, currentStateDuration);
                attributesResponseList.Add(attributeResponse);
            }

            var deviceResponse = new GetDeviceStateResponse(device.Key, attributesResponseList);
            devicesResponseList.Add(deviceResponse);
        }

        return devicesResponseList;
    }
}
