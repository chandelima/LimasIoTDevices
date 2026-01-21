using LimasIotDevices.Application.BackgroundServices;
using LimasIotDevices.Application.Services.Device;
using LimasIotDevices.Application.Services.Event;
using LimasIotDevices.Application.Services.HomeAssistant;
using LimasIotDevices.Application.Services.Translation;
using LimasIotDevices.Application.UseCases.DeviceAttributes;
using LimasIotDevices.Application.UseCases.Devices;
using LimasIoTDevices.Facade.Services;
using LimasIoTDevices.Facade.UseCases.DeviceAttributes;
using LimasIoTDevices.Facade.UseCases.Devices;
using Microsoft.Extensions.DependencyInjection;

namespace LimasIotDevices.Application;

public static class SetupApplication
{
    public static void AddAplication(this IServiceCollection services)
    {
        services.AddHostedService<HomeAssistantWebsocketConnectionWorker>();

        // Services
        services.AddScoped<ValidateDeviceService>();
        services.AddScoped<ConvertDeviceService>();
        services.AddScoped<GetExistentHomeAssistantDevicesService>();
        services.AddScoped<SendDeviceEventService>();
        services.AddSingleton<IUserEventService, UserEventService>();

        services.AddScoped<ISetUserTranslationService, SetUserTranslationService>();


        // Device use cases
        services.AddScoped<ISearchDevicesUseCase, SearchDevicesUseCase>();
        services.AddScoped<IGetDeviceByKeyUseCase, GetDeviceByKeyUseCase>();
        services.AddScoped<ICreateDevicesUseCase, CreateDevicesUseCase>();
        services.AddScoped<IUpdateDevicesUseCase, UpdateDevicesUseCase>();
        services.AddScoped<IRemoveDevicesUseCase, RemoveDevicesUseCase>();

        // Device attributes use cases
        services.AddScoped<ICallServiceForDeviceAttributeUseCase, CallServiceForDeviceAttributeUseCase>();
    }
}
