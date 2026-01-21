using LimasIotDevices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LimasIotDevices.Infrastructure.Data.EntityFrameworkMappings;

public class DeviceAttributeMap : IEntityTypeConfiguration<DeviceAttributeEntity>
{
    public void Configure(EntityTypeBuilder<DeviceAttributeEntity> builder)
    {
        builder.ToTable("device_attributes");

        builder.HasKey(x => new { x.DeviceKey, x.Key });

        builder.Property(x => x.DeviceKey).IsRequired().HasMaxLength(255).HasColumnName("device_key");
        builder.Property(x => x.Key).IsRequired().HasMaxLength(255).HasColumnName("key");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).HasColumnName("name");
        builder.Property(x => x.Description).HasMaxLength(500).HasColumnName("description");
        builder.Property(x => x.Entities).HasColumnType("varchar(255)[]").IsRequired().HasColumnName("entities");
        builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
    }
}
