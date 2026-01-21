using System.Text.RegularExpressions;

namespace LimasIoTDevices.Shared.Extensions;

public static class StringValidateExtensions
{
    public static bool HasSpecialCharacter(this string value)
    {
        return Regex.IsMatch(value, @"[^A-Za-z0-9]");
    }
}
