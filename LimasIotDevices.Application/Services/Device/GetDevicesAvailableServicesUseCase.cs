using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.UseCases.Devices;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.Services.Device;

internal class GetDevicesAvailableServicesUseCase(
    LimasIotDevicesDbContext dbContext,
    IHomeAssistantGateway homeAssistantGateway) : UseCaseBase, IGetDevicesAvailableServicesUseCase
{
    public async Task<List<GetDeviceAvailableServicesResponse>> Execute()
    {
        var databaseDevicesTask = dbContext.Devices.Include(x => x.Attributes).ToListAsync();
        var availableServicesTask = homeAssistantGateway.GetAvailableServices();

        await Task.WhenAll(databaseDevicesTask, availableServicesTask);

        var databaseDevices = await databaseDevicesTask;
        var availableServices = await availableServicesTask;

        var devicesResponseList = new List<GetDeviceAvailableServicesResponse>();

        foreach (var device in databaseDevices)
        {
            var attributesResponseList = new List<GetDeviceAttributeAvailableServicesResponse>();

            foreach (var attribute in device.Attributes)
            {
                var servicesForAttribute = new HashSet<string>();

                foreach (var entityId in attribute.Entities)
                {
                    var dotIndex = entityId.IndexOf('.');
                    if (dotIndex <= 0)
                        continue;

                    var domain = entityId[..dotIndex];

                    if (availableServices.TryGetValue(domain, out var domainServices))
                    {
                        foreach (var service in domainServices)
                        {
                            servicesForAttribute.Add(service);
                        }
                    }
                }

                var attributeResponse = new GetDeviceAttributeAvailableServicesResponse(
                    attribute.Key,
                    [.. servicesForAttribute.OrderBy(s => s)]
                );

                attributesResponseList.Add(attributeResponse);
            }

            var deviceResponse = new GetDeviceAvailableServicesResponse(device.Key, attributesResponseList);
            devicesResponseList.Add(deviceResponse);
        }

        return devicesResponseList;
    }
}
