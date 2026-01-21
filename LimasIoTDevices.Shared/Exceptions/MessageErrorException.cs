namespace LimasIoTDevices.Shared.Exceptions;

public class MessageErrorException : Exception
{
    private readonly List<string> _errors = new();

    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public MessageErrorException(IEnumerable<string>? errors = null, Exception? inner = null)
        : base(BuildMessage(errors), inner)
    {
        if (errors != null)
        {
            _errors.AddRange(errors);
        }
    }

    public MessageErrorException(string error, Exception? inner = null)
        : this(new[] { error }, inner) { }

    private static string BuildMessage(IEnumerable<string>? errors)
    {
        if (errors == null || !errors.Any())
        {
            return "Message error exception";
        }

        return string.Join("; ", errors);
    }
}
