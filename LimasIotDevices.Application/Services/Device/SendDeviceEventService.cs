using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.Enumerators;
using LimasIoTDevices.Facade.Services;
using LimasIoTDevices.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LimasIotDevices.Application.Services.Device;

internal class SendDeviceEventService(
    LimasIotDevicesDbContext _dbContext,
    IUserEventService _userEventService,
    IMemoryCache _memoryCache,
    IGetConfigurationService _configService)
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

        var hash = GetHash(databaseDevice.Key, eventAttributeKey, newStatus);
        if (!_memoryCache.TryGetValue(hash, out _))
        {
            var debounceMilliseconds = _configService.Execute<int>("DeviceEventDebounceMilliseconds");
            _memoryCache.Set(hash, true, TimeSpan.FromMilliseconds(debounceMilliseconds));
        }
        else
        {
            return;
        }

        var eventResponse = new EventResponse(EnumEventType.DeviceStateChanged, new DeviceStateChangedResponse(databaseDevice.Key, eventAttributeKey, newStatus));
        _userEventService.Broadcast(eventResponse);
    }

    private string GetHash(string deviceKey, string deviceAttribute, string newStatus)
    {
        return $"{nameof(SendDeviceEventService)}:{deviceKey}_{deviceAttribute}_{newStatus}";
    }
}
