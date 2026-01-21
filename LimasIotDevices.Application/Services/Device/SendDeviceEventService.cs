using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.Enumerators;
using LimasIoTDevices.Facade.Services;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.Services.Device;

internal class SendDeviceEventService(
    LimasIotDevicesDbContext _dbContext,
    IUserEventService _userEventService)
{
    public async Task Execute(string haEntity, string newStatus)
    {
        var databaseDevice = await _dbContext.Devices
            .Include(x => x.Attributes)
            .Where(x => x.Attributes.Any(a => a.Entities.Any(e => e.ToLower() == haEntity.ToLower())))
            .FirstOrDefaultAsync();

        if (databaseDevice is null) return;

        var eventAttributeKey = databaseDevice.Attributes
            .FirstOrDefault(a => a.Entities.Any(e => e.ToLower() == haEntity.ToLower()))!
            .Key;

        var eventResponse = new EventResponse(EnumEventType.DeviceStateChanged, new DeviceStateChangedResponse(databaseDevice.Key, eventAttributeKey, newStatus));
        _userEventService.Broadcast(eventResponse);
    }
}
