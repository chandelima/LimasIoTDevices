using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.UseCases.Devices;
using LimasIoTDevices.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.UseCases.Devices;

internal class RemoveDevicesUseCase : UseCaseBase, IRemoveDevicesUseCase
{
    private readonly LimasIotDevicesDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveDevicesUseCase(LimasIotDevicesDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Execute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        var databaseDevice = await _dbContext.Devices
            .Include(x => x.Attributes)
            .FirstOrDefaultAsync(x => x.Key == key);

        if (databaseDevice is null) return false;

        _dbContext.Devices.Remove(databaseDevice);
        var result = await _unitOfWork.SaveChangesAsync();

        return result > 0;
    }
}
