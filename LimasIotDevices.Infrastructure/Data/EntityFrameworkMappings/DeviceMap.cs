using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimasIotDevices.Domain.Entities;

namespace LimasIotDevices.Infrastructure.Data.EntityFrameworkMappings;

public class DeviceMap : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.ToTable("devices");

        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(255).HasColumnName("key");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).HasColumnName("name");
        builder.Property(x => x.Description).HasMaxLength(500).HasColumnName("description");
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");

        builder.HasMany(x => x.Attributes)
               .WithOne()
               .HasForeignKey(x => x.DeviceKey);
    }
}
