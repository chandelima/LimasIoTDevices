using LimasIoTDevices.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LimasIoTDevices.Shared;

public static class SetupShared
{
    public static void AddShared(this IServiceCollection services)
    {
        services.AddSingleton<IGetConfigurationService, GetConfigurationService>();
    }
}
