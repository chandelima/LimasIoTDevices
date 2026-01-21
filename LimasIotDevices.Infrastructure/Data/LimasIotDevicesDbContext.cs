using LimasIotDevices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Infrastructure.Data;

public class LimasIotDevicesDbContext : DbContext
{
    public DbSet<DeviceEntity> Devices { get; set; }
    public DbSet<DeviceAttributeEntity> Attributes { get; set; }

    public LimasIotDevicesDbContext(DbContextOptions<LimasIotDevicesDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LimasIotDevicesDbContext).Assembly);
    }
}
