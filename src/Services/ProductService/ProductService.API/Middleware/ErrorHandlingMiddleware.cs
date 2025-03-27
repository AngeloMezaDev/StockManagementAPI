using System.Net;
using TransactionService.API.Models;
using TransactionService.Domain.Exceptions;

namespace ProductService.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var response = new ErrorResponses { Message = "An unexpected error occurred" };

        switch (exception)
        {
            case EntityNotFoundExceptions ex:
                statusCode = HttpStatusCode.NotFound;
                response.Message = ex.Message;
                break;

            case DatabaseExceptions ex:
                statusCode = HttpStatusCode.ServiceUnavailable;
                response.Message = "A database error occurred";
                _logger.LogError(ex, ex.Message);
                break;

            case InvalidOperationException ex:
                statusCode = HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                _logger.LogWarning(ex, "Business validation failed: {Message}", ex.Message);
                break;

            case ArgumentException ex:
                statusCode = HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                break;

            default:
                _logger.LogError(exception, "Unhandled exception");
                // Puedes añadir el TraceId para depuración
                response.TraceId = context.TraceIdentifier;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}