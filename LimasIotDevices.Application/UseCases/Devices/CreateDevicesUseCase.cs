using LimasIotDevices.Application.Base.UseCases;
using LimasIotDevices.Application.Services.Device;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.UseCases.Devices;
using LimasIoTDevices.Shared.Data;

namespace LimasIotDevices.Application.UseCases.Devices;

internal class CreateDevicesUseCase : UseCaseBase, ICreateDevicesUseCase
{
    private readonly ValidateDeviceService _validateService;
    private readonly ConvertDeviceService _convertService;
    private readonly LimasIotDevicesDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDevicesUseCase(
        ValidateDeviceService validateService,
        ConvertDeviceService convertService,
        LimasIotDevicesDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _validateService = validateService;
        _convertService = convertService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Execute(CreateUpdateDeviceRequest request)
    {
        await _validateService.Execute(request);

        var deviceToSave = _convertService.ToEntity(request);
        deviceToSave.Attributes = request.Attributes!.Select(_convertService.ToEntity).ToList();

        await _dbContext.Devices.AddAsync(deviceToSave);
        var result = await _unitOfWork.SaveChangesAsync();

        return result > 0;
    }
}
