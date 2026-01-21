using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.Enumerators;
using LimasIoTDevices.Facade.Services;
using LimasIoTDevices.Shared.Attributes;
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

        Func<EventResponse, Task> onMessage = async (EventResponse message) =>
        {
            try
            {
                var json = message.Data.ConvertToJson();

                string? eventType = null;

                if (message is EventResponse ev)
                {
                    eventType = ev.Event.GetStringValue().ToLowerInvariant();
                }

                var ssePayload = "";

                if (!string.IsNullOrEmpty(eventType))
                {
                    ssePayload += $"event: {eventType}\n";
                }

                var dataLines = json.Split('\n').Select(line => $"data: {line}");
                ssePayload += string.Join("\n", dataLines);
                ssePayload += "\n\n";

                await response.WriteAsync(ssePayload);
                await response.Body.FlushAsync();
            }
            catch (ObjectDisposedException)
            {
                // Response already disposed, client disconnected
            }
            catch (InvalidOperationException)
            {
                // Response is no longer valid
            }
        };


        _ = onMessage(new EventResponse(
            EnumEventType.ConnectionEstablished,
            new { Message = "Connection established successfully!" }));

        var subscription = _subject.Subscribe(async msg => await onMessage(msg),
            onError: _ => { /* Ignore errors */ },
            onCompleted: () => { /* Stream completed */ });

        return subscription;
    }

    public void Broadcast(EventResponse message)
    {
        _subject.OnNext(message);
    }
}