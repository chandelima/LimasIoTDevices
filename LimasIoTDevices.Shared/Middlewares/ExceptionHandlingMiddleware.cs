using LimasIoTDevices.Shared.DTOs;
using LimasIoTDevices.Shared.Exceptions;
using LimasIoTDevices.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LimasIoTDevices.Shared.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private readonly List<MessageResponse> _messageList = new();

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (MessageErrorException ex)
        {
            context.Response.StatusCode = 400;

            foreach (var error in ex.Errors)
            {
                _messageList.AddError(error);
            }

            await HandleException(context, ex);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            _messageList.AddError($"Ocorreu um erro desconhecido. ({context.TraceIdentifier})");

            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex, bool writeResponse = true)
    {
        var message = $"--- TraceIdentifier: {context.TraceIdentifier} ---\n{ex.Message}";
        _logger.LogError(ex, message);

        if (ex.InnerException != null)
        {
            await HandleException(context, ex.InnerException, false);
        }

        var response = new { messages = _messageList };

        if (writeResponse)
        {
            context.Response.ContentType = "application/json";
            var jsonResponse = response.ConvertToJson();

            await context.Response.WriteAsync(jsonResponse);
        }

        _messageList.Clear();
    }
}
