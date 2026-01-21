using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Infrastructure.Data;
using LimasIotDevices.Infrastructure.Data.Migrations;
using LimasIotDevices.Infrastructure.External;
using LimasIotDevices.Infrastructure.Handlers;
using LimasIoTDevices.Shared.Data;
using LimasIoTDevices.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;


namespace LimasIotDevices.Infrastructure;

public static class SetupInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabase(services, configuration);
        InjectDependencies(services);
    }

    public static void UseInfrastructureSettings(this IApplicationBuilder app, IConfiguration configuration)
    {
        ApplyMigrations(app, configuration);
    }

    private static void InjectDependencies(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<LimasIotDevicesDbContext>>();
        services.AddTransient<HomeAssistantTokenDelegatingHandler>();

        services.AddHttpClient<IHomeAssistantGateway, HomeAssistantGateway>((sp, client) =>
        {
            var configService = sp.GetRequiredService<IGetConfigurationService>();
            var uriString = configService.Execute<string>("HomeAssistantData:HostUrl");

            client.BaseAddress = new Uri(uriString);
        }).AddHttpMessageHandler<HomeAssistantTokenDelegatingHandler>();
    }

    private static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        long.TryParse(configuration["RevertToTargetVersion"], out long revertToTargetVersion);

        if (revertToTargetVersion is not 0)
        {
            runner.MigrateDown(revertToTargetVersion);
        }
        else
        {
            runner.MigrateUp();
        }

        return app;
    }

    private static void SetupDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LimasIotDevices");

        services.AddDbContext<LimasIotDevicesDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableDetailedErrors();

            if (Debugger.IsAttached)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(cfg => cfg
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(FluentMigrationVersionTable).Assembly)
                .For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddScoped<IVersionTableMetaData, FluentMigrationVersionTable>();
    }
}
