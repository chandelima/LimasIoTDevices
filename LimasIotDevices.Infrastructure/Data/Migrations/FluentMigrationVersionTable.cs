using FluentMigrator.Runner.VersionTableInfo;

namespace LimasIotDevices.Infrastructure.Data.Migrations;

public class FluentMigrationVersionTable : IVersionTableMetaData
{
    public object ApplicationContext { get; set; } = null!;
    public string AppliedOnColumnName => "applied_on";
    public string ColumnName => "version";
    public string DescriptionColumnName => "migration";
    public bool OwnsSchema => false;
    public string SchemaName => string.Empty;
    public string TableName => "__migrations_metadata";
    public string UniqueIndexName => string.Empty;
    public bool CreateWithPrimaryKey => true;
}
