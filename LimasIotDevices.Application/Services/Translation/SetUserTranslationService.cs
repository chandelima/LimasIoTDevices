using LimasIotDevices.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace LimasIotDevices.Application.Services.Translation;

public interface ISetUserTranslationService
{
    Task Execute();
}

internal class SetUserTranslationService : ISetUserTranslationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _environment;

    public SetUserTranslationService(IHttpContextAccessor httpContextAccessor, IHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    public async Task Execute()
    {
        var userLanguage = "en-US";
        var path = Path.Combine(
            AppContext.BaseDirectory, 
            "Translations", 
            $"{userLanguage}.json");

        try
        {
            var json = await File.ReadAllTextAsync(path, _httpContextAccessor.HttpContext.RequestAborted);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var localizationModel = JsonSerializer.Deserialize<TranslationModel>(json, options) ?? new TranslationModel();

            _httpContextAccessor.HttpContext.Items[nameof(TranslationModel)] = localizationModel;
        }
        catch
        {
            _httpContextAccessor.HttpContext.Items[nameof(TranslationModel)] = new TranslationModel();
        }
    }
}
