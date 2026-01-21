namespace LimasIotDevices.Domain.Entities;

public class DeviceAttributeEntity
{
    public string DeviceKey { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string[] Entities { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
}
