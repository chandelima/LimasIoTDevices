using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Facade.UseCases.DeviceAttributes;
using LimasIoTDevices.Facade.UseCases.Devices;
using LimasIoTDevices.Shared.DTOs;
using LimasIoTDevices.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LimasIoTDevices.API.Controllers;

[ApiController]
[Route("api/v1/devices")]
public class DeviceController : ControllerBase
{
    [HttpGet("states")]
    public async Task<ActionResult<DefaultControllerResponse<List<GetDeviceStateResponse>>>> GetState(
        [FromServices] IGetDevicesStatesUseCase useCase)
    {
        var result = await useCase.Execute();
        return this.GetResponse(result, useCase);
    }

    [HttpGet("{deviceKey:required}/state/{attributeKey?}")]
    public async Task<ActionResult<DefaultControllerResponse<GetDeviceStateResponse>>> GetDeviceState(
        [FromRoute] string deviceKey,
        [FromRoute] string? attributeKey,
        [FromServices] IGetDevicesStateUseCase useCase)
    {
        var result = await useCase.Execute(deviceKey, attributeKey);
        return this.GetResponse(result, useCase);
    }

    [HttpGet("available-services")]
    public async Task<ActionResult<DefaultControllerResponse<List<GetDeviceAvailableServicesResponse>>>> GetAvailableServices(
        [FromServices] IGetDevicesAvailableServicesUseCase useCase)
    {
        var result = await useCase.Execute();
        return this.GetResponse(result, useCase);
    }

    [HttpGet("search")]
    public async Task<ActionResult<DefaultControllerResponse<List<GetDeviceResponse>>>> Search(
    [FromQuery] string? searchTerm,
    [FromServices] ISearchDevicesUseCase useCase)
    {
        var result = await useCase.Execute(searchTerm);
        return this.GetResponse(result, useCase);
    }

    [HttpGet("{key:required}")]
    public async Task<ActionResult<DefaultControllerResponse<GetDeviceResponse>>> GetByKey(
    [FromRoute] string key,
    [FromServices] IGetDeviceByKeyUseCase useCase)
    {
        var result = await useCase.Execute(key);
        return this.GetResponse(result, useCase);
    }

    [HttpPost]
    public async Task<ActionResult<DefaultControllerResponse<bool>>> Create(
        [FromBody] CreateUpdateDeviceRequest request,
        [FromServices] ICreateDevicesUseCase useCase)
    {
        var result = await useCase.Execute(request);
        return this.CreateResponse(result, useCase);
    }

    [HttpPut]
    public async Task<ActionResult<DefaultControllerResponse<bool>>> Update(
        [FromBody] CreateUpdateDeviceRequest request,
        [FromServices] IUpdateDevicesUseCase useCase)
    {
        var result = await useCase.Execute(request);
        return this.CreateResponse(result, useCase);
    }

    [HttpDelete("{key:required}")]
    public async Task<ActionResult> Delete(
        [FromRoute] string key,
        [FromServices] IRemoveDevicesUseCase useCase)
    {
        var result = await useCase.Execute(key);
        return this.DeleteResponse(result, useCase.Messages);
    }

    [HttpPost("{deviceKey:required}/{attributeKey:required}/call-service/{service}")]
    public async Task<ActionResult<DefaultControllerResponse<bool>>> CallService(
        [FromServices] ICallServiceForDeviceAttributeUseCase useCase,
        [FromRoute] string deviceKey,
        [FromRoute] string service,
        [FromRoute] string? attributeKey = null,
        [FromBody] Dictionary<string, object?>? requestData = null)
    {
        var result = await useCase.Execute(deviceKey, attributeKey, service, requestData);
        return new DefaultControllerResponse<bool>(result);
    }

    [HttpPost("{deviceKey:required}/call-service/{service}")]
    public async Task<ActionResult<DefaultControllerResponse<bool>>> CallService(
        [FromServices] ICallServiceForDeviceAttributeUseCase useCase,
        [FromRoute] string deviceKey,
        [FromRoute] string service,
        [FromBody] Dictionary<string, object?>? requestData)
    {
        var result = await useCase.Execute(deviceKey, null, service, requestData);
        return new DefaultControllerResponse<bool>(result);
    }
}
