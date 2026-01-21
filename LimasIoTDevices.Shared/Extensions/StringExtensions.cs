namespace LimasIoTDevices.Shared.Extensions;

public static class StringExtensions
{
    public static List<string> GetDuplicated(this IEnumerable<string> origin)
    {
        return origin
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
    }
}
