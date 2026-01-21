using FluentMigrator;

namespace LimasIotDevices.Infrastructure.Data.Migrations;

[Migration(2026010700)]
public class CreateDevicesMigration : Migration
{
    public override void Up()
    {
        Create.Table("devices")
            .WithColumn("key").AsString(255).PrimaryKey().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(500).Nullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();

        Create.Index("ix_devices_name")
            .OnTable("devices")
            .OnColumn("name").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Index("ix_devices_name").OnTable("devices");
        Delete.Table("devices");
    }
}
