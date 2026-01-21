using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using LimasIoTDevices.Shared.Attributes;

namespace LimasIoTDevices.Shared.Converters.EnumStringValue;

public class EnumStringValueConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert.IsEnum)
        {
            return HasStringValueAttribute(typeToConvert);
        }

        var underlying = Nullable.GetUnderlyingType(typeToConvert);
        if (underlying != null && underlying.IsEnum)
        {
            return HasStringValueAttribute(underlying);
        }

        return false;
    }

    private static bool HasStringValueAttribute(Type enumType)
    {
        return enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                       .Any(f => f.GetCustomAttribute<StringValueAttribute>() != null);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlying = Nullable.GetUnderlyingType(typeToConvert);
        if (underlying != null && underlying.IsEnum)
        {
            var converterType = typeof(NullableEnumStringValueConverter<>).MakeGenericType(underlying);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        var nonNullableConverterType = typeof(EnumStringValueConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(nonNullableConverterType)!;
    }
}

public class EnumStringValueConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for enum {typeof(T)}, got {reader.TokenType}");
        }

        string? value = reader.GetString();
        if (value == null)
        {
            throw new JsonException($"Null value for enum {typeof(T)}");
        }

        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<StringValueAttribute>();
            if (attr != null && attr.Value == value)
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new JsonException($"Unknown string value '{value}' for enum {typeof(T)}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var field = typeof(T).GetField(value.ToString());
        if (field == null)
        {
            throw new JsonException($"Invalid enum value {value} for {typeof(T)}");
        }

        var attr = field.GetCustomAttribute<StringValueAttribute>();
        if (attr != null)
        {
            writer.WriteStringValue(attr.Value);
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

public class NullableEnumStringValueConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    private readonly EnumStringValueConverter<T> _inner = new();

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return _inner.Read(ref reader, typeof(T), options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        _inner.Write(writer, value.Value, options);
    }
}