using LimasIoTDevices.Facade.Services;
using Microsoft.AspNetCore.Mvc;

namespace LimasIoTDevices.API.Controllers;

[Route("api/v1/events")]
[ApiController]
public class EventController : ControllerBase
{
    [HttpGet("subscribe")]
    public async Task Subscribe(
        [FromServices] IUserEventService notifyerService)
    {
        notifyerService.Subscribe(Response);

        try
        {
            await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted);
        }
        catch (OperationCanceledException)
        {
            // The request was cancelled (client disconnected).
        }
    }
}
