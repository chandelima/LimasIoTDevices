using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Application.Services.Device;
using LimasIotDevices.Domain.Entities;
using LimasIotDevices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.UseCases.Devices;

internal class SearchDevicesUseCase(
    LimasIotDevicesDbContext _dbContext,
    ConvertDeviceService _convertDeviceService) : UseCaseBase, LimasIoTDevices.Facade.UseCases.Devices.ISearchDevicesUseCase
{
    public async Task<List<LimasIoTDevices.Facade.Dtos.GetDeviceResponse>> Execute(string? searchTerm)
    {
        var lowerSearchterm = searchTerm?.Trim().ToLower();

        List<DeviceEntity> databaseDevices;

        if (string.IsNullOrWhiteSpace(lowerSearchterm))
        {
            databaseDevices = await _dbContext.Devices
                .Include(x => x.Attributes)
                .ToListAsync();
        }
        else
        {
            databaseDevices = await _dbContext.Devices
                .Include(x => x.Attributes)
                .Where(x => x.Key.ToLower().Contains(lowerSearchterm)
                         || x.Name.ToLower().Contains(lowerSearchterm)
                         || x.Attributes.Any(x => x.Key.Contains(lowerSearchterm)
                                               || x.Name.Contains(lowerSearchterm)))
                .ToListAsync();
        }

        return databaseDevices
            .Select(_convertDeviceService.ToResponse)
            .ToList();
    }
}
