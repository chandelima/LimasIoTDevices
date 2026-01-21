namespace LimasIoTDevices.Shared.Attributes;

using System;
using System.Collections.Concurrent;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class StringValueAttribute : Attribute
{
    public string Value { get; }

    public StringValueAttribute(string value)
    {
        Value = value;
    }
}


public static class StringValueEnumExtensions
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, object>> FromStringCache = new();
    private static readonly ConcurrentDictionary<Type, Dictionary<object, string>> ToStringCache = new();

    public static string GetStringValue(this Enum value)
    {
        var type = value.GetType();
        var map = ToStringCache.GetOrAdd(type, BuildToStringMap);

        return map.TryGetValue(value, out var result)
            ? result
            : value.ToString();
    }

    public static TEnum ConvertFromStringValueTo<TEnum>(this string value)
        where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        var map = FromStringCache.GetOrAdd(type, BuildFromStringMap);

        if (map.TryGetValue(value, out var enumValue))
        {
            return (TEnum)enumValue;
        }

        throw new ArgumentException(
            $"Value '{value}' is not valid for enum {type.Name}");
    }

    private static Dictionary<object, string> BuildToStringMap(Type enumType)
    {
        var dict = new Dictionary<object, string>();

        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumValue = field.GetValue(null)!;
            var attr = field.GetCustomAttribute<StringValueAttribute>();

            dict[enumValue] = attr?.Value ?? field.Name;
        }

        return dict;
    }

    private static Dictionary<string, object> BuildFromStringMap(Type enumType)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumValue = field.GetValue(null)!;
            var attr = field.GetCustomAttribute<StringValueAttribute>();

            var key = attr?.Value ?? field.Name;

            if (!dict.TryAdd(key, enumValue))
            {
                throw new InvalidOperationException(
                    $"Duplicate StringValue '{key}' in enum {enumType.Name}");
            }
        }

        return dict;
    }
}

