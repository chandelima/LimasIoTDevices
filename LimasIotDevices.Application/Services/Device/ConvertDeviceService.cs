using LimasIotDevices.Domain.Entities;
using LimasIoTDevices.Facade.Dtos;

namespace LimasIotDevices.Application.Services.Device;

internal class ConvertDeviceService
{
    public DeviceEntity ToEntity(CreateUpdateDeviceRequest request)
    {
        var entity = new DeviceEntity();

        entity.Key = request.Key!.Trim();
        entity.Name = request.Name!.Trim();
        entity.Description = request.Description?.Trim();
        entity.CreatedAt = DateTimeOffset.UtcNow;

        return entity;
    }

    public DeviceAttributeEntity ToEntity(CreateUpdateDeviceAttributeRequest request)
    {
        var entity = new DeviceAttributeEntity();

        entity.Key = request.Key!.Trim();
        entity.Name = request.Name!.Trim();
        entity.Description = request.Description?.Trim();
        entity.Entities = request?.Entities!;
        entity.CreatedAt = DateTimeOffset.UtcNow;

        return entity;
    }

    public GetDeviceResponse ToResponse(DeviceEntity entity)
    {
        return new GetDeviceResponse(
            entity.Key,
            entity.Name,
            entity.Description,
            entity.Attributes.Select(
                x => new GetDeviceAttributeResponse(
                    x.Key,
                    x.Name,
                    x.Description,
                    x.Entities)).ToList());
    }

    public void UpdateEntity(
        DeviceAttributeEntity entity, 
        CreateUpdateDeviceAttributeRequest request)
    {
        entity.Key = request.Key!;
        entity.Name = request.Name!;
        entity.Description = request.Description;
        entity.Entities = request.Entities ?? [];
    }
}
