using LimasIotDevices.Application.Services.Translation;

namespace LimasIoTDevices.API.Middlewares;

public sealed class SetRequestTranslationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public SetRequestTranslationMiddleware(
        RequestDelegate next,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var setUserTranslationService = scope.ServiceProvider.GetRequiredService<ISetUserTranslationService>();

        await setUserTranslationService.Execute();
        await _next(context);
    }
}
