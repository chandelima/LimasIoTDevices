using FluentMigrator;
using System.Data;

namespace LimasIotDevices.Infrastructure.Data.Migrations;

[Migration(2026010701)]
public class CreateDeviceAttributesMigration : Migration
{
    public override void Up()
    {
        Create.Table("device_attributes")
            .WithColumn("device_key").AsString(255).NotNullable()
            .WithColumn("key").AsString(255).NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(500).Nullable()
            .WithColumn("entities").AsCustom("varchar(255)[]").NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();

        Create.PrimaryKey("pk_device_attributes").OnTable("device_attributes").Columns([ "device_key", "key" ]);

        Create.Index("ix_device_attributes_device_key")
            .OnTable("device_attributes")
            .OnColumn("device_key").Ascending()
            .WithOptions().NonClustered();

        Create.ForeignKey("fk_device_attributes_device")
            .FromTable("device_attributes").ForeignColumn("device_key")
            .ToTable("devices").PrimaryColumn("key")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("fk_device_attributes_device").OnTable("device_attributes");
        Delete.Index("ix_device_attributes_device_key").OnTable("device_attributes");
        Delete.PrimaryKey("pk_device_attributes").FromTable("device_attributes");
        Delete.Table("device_attributes");
    }
}
