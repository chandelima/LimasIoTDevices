namespace LimasIotDevices.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
