using LimasIoTDevices.Contracts.Dtos;
using Microsoft.AspNetCore.Http;

namespace LimasIoTDevices.Contracts.Services;

public interface IUserEventService
{
    void Broadcast(EventResponse message);
    IDisposable Subscribe(HttpResponse response);
}