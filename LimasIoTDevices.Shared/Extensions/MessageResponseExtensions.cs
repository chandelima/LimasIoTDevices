using LimasIoTDevices.Shared.DTOs;
using LimasIoTDevices.Shared.Enumerators;

namespace LimasIoTDevices.Shared.Extensions;

public static class MessageResponseExtensions
{
    public static void AddError(this List<MessageResponse> list, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException("Mensagem do erro deve ser fornecida.");
        }

        var response = new MessageResponse(EnumResponseMessageType.Error, message);

        list.Add(response);
    }
}
