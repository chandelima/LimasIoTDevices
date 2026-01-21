using LimasIoTDevices.Shared.Converters.EnumStringValue;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LimasIoTDevices.Shared.Extensions;

public static class StringConvertExtensions
{
    public static string ConvertToAlphanumericOnly(this string? value)
    {
        value ??= "";
        return new string(value.Where(char.IsLetterOrDigit).ToArray());
    }

    public static string ConvertToNumericOnly(this string? value)
    {
        value ??= "";
        return new string(value.Where(char.IsDigit).ToArray());
    }

    public static string ConvertToAlphabeticOnly(this string? value)
    {
        value ??= "";
        return new string(value.Where(char.IsDigit).ToArray());
    }

    public static string ConvertToJson(this object? obj, JsonSerializerOptions? options = null)
    {
        if (obj == null)
        {
            return "{}";
        }

        options ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        if (!options.Converters.Any(c => c is EnumStringValueConverterFactory))
        {
            options.Converters.Add(new EnumStringValueConverterFactory());
        }

        return JsonSerializer.Serialize(obj, options);
    }

    public static T? ConvertFromJson<T>(this string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        options ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(json, options);
    }
}
