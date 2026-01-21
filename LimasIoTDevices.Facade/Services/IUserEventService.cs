using LimasIoTDevices.Facade.Dtos;
using Microsoft.AspNetCore.Http;

namespace LimasIoTDevices.Facade.Services;

public interface IUserEventService
{
    void Broadcast(EventResponse message);
    IDisposable Subscribe(HttpResponse response);
}