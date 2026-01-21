namespace LimasIoTDevices.Shared.DTOs;

public record DefaultControllerResponse<T>
{
    public DefaultControllerResponse(
        T? data,
        List<MessageResponse>? messages = null)
    {
        Data = data;
        if (messages is not null)
        {
            Messages = messages;
        }
    }

    public T? Data { get; init; } = default!;
    public List<MessageResponse> Messages { get; init; } = [];
    public bool Ok { get => !Messages.Any(x => x.Type == Enumerators.EnumResponseMessageType.Error); }
}
