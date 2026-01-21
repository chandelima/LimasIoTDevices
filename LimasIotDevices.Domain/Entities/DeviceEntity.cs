namespace LimasIotDevices.Domain.Entities;

public class DeviceEntity
{
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<DeviceAttributeEntity> Attributes { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
}
