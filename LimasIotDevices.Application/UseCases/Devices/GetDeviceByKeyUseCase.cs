using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Application.Services.Device;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Contracts.Dtos;
using LimasIoTDevices.Contracts.UseCases.Devices;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.UseCases.Devices;

internal class GetDeviceByKeyUseCase(
    LimasIotDevicesDbContext _dbContext,
    ConvertDeviceService _convertDeviceService) : UseCaseBase, IGetDeviceByKeyUseCase
{
    public async Task<GetDeviceResponse?> Execute(string key)
    {
        var databaseDevice = await _dbContext.Devices
            .AsNoTracking()
            .Include(x => x.Attributes)
            .FirstOrDefaultAsync(x => x.Key.ToLower().Equals(key.ToLower()));

        if (databaseDevice is null) return null;

        return _convertDeviceService.ToResponse(databaseDevice);
    }
}
