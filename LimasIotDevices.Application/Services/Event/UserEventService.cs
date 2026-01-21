using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.Enumerators;
using LimasIoTDevices.Facade.Services;
using LimasIoTDevices.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using System.Reactive.Subjects;

namespace LimasIotDevices.Application.Services.Event;

public class UserEventService : IUserEventService
{
    // ToDo: check if Channel is better than Subject here
    private readonly Subject<EventResponse> _subject = new();

    public IDisposable Subscribe(HttpResponse response)
    {
        response.Headers.Append("Content-Type", "text/event-stream");
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("Connection", "keep-alive");

        Func<object, Task> onMessage = async (object message) =>
        {
            await response.WriteAsync($"{message.ConvertToJson()}{Environment.NewLine}");
            await response.Body.FlushAsync();
        };

        _ = onMessage(new EventResponse(
            EnumEventType.ConnectionEstablished,
            new { Message = "Connection established successfully!" }));

        return _subject.Subscribe(async msg => await onMessage(msg));
    }

    public void Broadcast(EventResponse message)
    {
        _subject.OnNext(message);
    }
}