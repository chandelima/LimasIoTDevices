using LimasIotDevices.Domain.Models;
using Microsoft.OpenApi.Models;
using LimasIoTDevices.Shared.Extensions;
using LimasIoTDevices.API.Middlewares;
using LimasIoTDevices.Shared.Middlewares;

namespace LimasIoTDevices.API;

public static class SetupAPI
{
    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddScoped(sp =>
        {
            var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (http is not null && http.Items.TryGetValue(nameof(TranslationModel), out var model) && model is TranslationModel translationModel)
            {
                return translationModel;
            }

            return new TranslationModel();
        });

        services.ConfigureHttpResilienceHandler();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Limas IoT Devices API",
                Version = "v1",
            });
        });
    }

    public static void UseApi(this WebApplication app)
    {
        app.UseMiddleware<SetRequestTranslationMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json", 
                "Limas IoT Devices API v1");
        });

        app.UseCors(options =>
        {
            options.AllowAnyHeader();
            options.AllowAnyMethod();
            options.AllowAnyOrigin();
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
