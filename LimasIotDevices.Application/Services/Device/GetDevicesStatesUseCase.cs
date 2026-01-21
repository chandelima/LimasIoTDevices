using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LimasIoTDevices.Facade.UseCases.Devices;

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
                object state = "undefined";
                double currentStateDuration = 0;

                foreach (var entity in attribute.Entities)
                {
                    var haDeviceStatus = haGatewayDeviceStatus.FirstOrDefault(x => x.EntityId == entity);
                    if (haDeviceStatus.State != "undefined")
                    {
                        state = haDeviceStatus.State;
                        currentStateDuration = haDeviceStatus.CurrentStateDuration.TotalSeconds;

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
