using Microsoft.AspNetCore.Diagnostics;
using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Inventory;
namespace ProductInventory.Api.Errors;
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ValidationException => 400,
            BadHttpRequestException => 400,
            NotFoundException => 404,
            ConflictException => 409,
            StockConflictException => 409,
            _ => 500
        };
        var message = status switch
        {
            500 => "No se pudo completar la operación.",
            400 when exception is BadHttpRequestException => "El cuerpo o los parámetros de la solicitud no son válidos.",
            _ => exception.Message
        };
        if (status == 500)
            logger.LogError(exception, "Unhandled request failure.");
        context.Response.StatusCode = status;
        await Results.Problem(statusCode: status, title: message).ExecuteAsync(context);

        return true;
    }
}
