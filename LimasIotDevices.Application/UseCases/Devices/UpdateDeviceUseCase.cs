using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Application.Services.Device;
using LimasIotDevices.Domain.Entities;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.UseCases.Devices;
using LimasIoTDevices.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.UseCases.Devices;

internal class UpdateDevicesUseCase : UseCaseBase, IUpdateDevicesUseCase
{
    private readonly ConvertDeviceService _convertService;
    private readonly ValidateDeviceService _validateService;
    private readonly LimasIotDevicesDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDevicesUseCase(
        ConvertDeviceService convertService,
        ValidateDeviceService validateService,
        LimasIotDevicesDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _convertService = convertService;
        _validateService = validateService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Execute(CreateUpdateDeviceRequest request)
    {
        await _validateService.Execute(request, false);

        var databaseDevice = await _dbContext.Devices
            .Include(x => x.Attributes)
            .FirstOrDefaultAsync(x => x.Key == request.Key);

        if (databaseDevice is null) return false;

        databaseDevice.Name = request.Name!.Trim();
        databaseDevice.Description = request.Description?.Trim();

        SetDeviceAttributes(databaseDevice.Attributes, request.Attributes!);

        var result = await _unitOfWork.SaveChangesAsync();

        return result > 0;
    }

    private void SetDeviceAttributes(
        List<DeviceAttributeEntity> databaseAttributes, 
        List<CreateUpdateDeviceAttributeRequest> requestAttributes)
    {
        var entityAttributesToRemove = databaseAttributes
            .ExceptBy(requestAttributes.Select(x => x.Key), x => x.Key);
        foreach (var device in entityAttributesToRemove)
        {
            databaseAttributes.Remove(device);
        }

        foreach (var attribute in databaseAttributes)
        {
            var requestAttribute = requestAttributes.First(x => x.Key == attribute.Key);
            _convertService.UpdateEntity(attribute, requestAttribute);
        }

        var requestAttributesToInclude = requestAttributes
            .ExceptBy(databaseAttributes.Select(x => x.Key), x => x.Key);
        var entityAttributesToInclude = requestAttributesToInclude
            .Select(_convertService.ToEntity);
        databaseAttributes.AddRange(entityAttributesToInclude);
    }
}
