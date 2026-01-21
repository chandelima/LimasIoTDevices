namespace LimasIotDevices.Domain.Entities;

public class RoomEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
