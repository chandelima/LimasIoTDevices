using LimasIoTDevices.Shared.DTOs;
using LimasIoTDevices.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimasIoTDevices.Shared.Extensions;

public static class ControllerBaseExtensions
{
    public static ActionResult<DefaultControllerResponse<T>> CreateResponse<T>(this ControllerBase controller, T? data, List<MessageResponse>? messages = null)
    {
        var response = new DefaultControllerResponse<T>(data, messages);

        if (data is string stringData && !string.IsNullOrWhiteSpace(stringData) || data != null)
        {
            return controller.StatusCode(StatusCodes.Status201Created, response);
        }

        if (data is bool boolData && boolData || data is not null)
        {
            return controller.StatusCode(StatusCodes.Status201Created);
        }

        return controller.BadRequest(response);
    }

    public static ActionResult<DefaultControllerResponse<T>> CreateResponse<T>(this ControllerBase controller, T? data, IHasMessage hasMessage)
    {
        return CreateResponse(controller, data, hasMessage.Messages);
    }

    public static ActionResult<DefaultControllerResponse<T>> GetResponse<T>(this ControllerBase controller, T? data, List<MessageResponse>? messages = null)
    {
        var response = new DefaultControllerResponse<T>(data, messages);

        if (data != null)
        {
            return controller.Ok(response);
        }

        if (messages == null || messages.Count == 0)
        {
            return controller.NotFound();
        }

        return controller.BadRequest(response);
    }

    public static ActionResult<DefaultControllerResponse<T>> GetResponse<T>(this ControllerBase controller, T? data, IHasMessage hasMessage)
    {
        return GetResponse(controller, data, hasMessage.Messages);
    }

    public static ActionResult<DefaultControllerResponse<T>> SuccessResponse<T>(this ControllerBase controller, T? data, List<MessageResponse>? messages = null, bool forceOk = false)
    {
        var response = new DefaultControllerResponse<T>(data, messages);

        if (data != null || forceOk)
        {
            return controller.Ok(response);
        }

        return controller.BadRequest(response);
    }

    public static ActionResult<DefaultControllerResponse<T>> UpdateResponse<T>(this ControllerBase controller, T? data, List<MessageResponse>? messages = null)
    {
        var response = new DefaultControllerResponse<T>(data, messages);

        if (data is bool dataBool && dataBool is not false && data != null)
        {
            return controller.Ok(response);
        }

        return controller.BadRequest(response);
    }

    public static ActionResult DeleteResponse<T>(this ControllerBase controller, T? data, List<MessageResponse>? messages = null)
    {
        var response = new DefaultControllerResponse<T>(data, messages);

        if (data is bool retorno && retorno)
        {
            return controller.NoContent();
        }

        return controller.BadRequest(response);
    }
}
