using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NuamExchange.Application.Exceptions;

namespace NuamExchange.Api.Errors;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private const string ProblemJsonContentType = "application/problem+json";

    private static readonly Action<ILogger, string, Exception?> LogUnhandledException =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1001, "UnhandledException"),
            "Unhandled error {TraceId}");

    private static readonly Action<ILogger, int, string, string, Exception?> LogHandledException =
        LoggerMessage.Define<int, string, string>(
            LogLevel.Warning,
            new EventId(1002, "HandledException"),
            "Handled error {Status} {TraceId}: {Message}");

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail, errors) = exception switch
        {
            ValidationException validationException => (400, "Solicitud inválida", validationException.Message, validationException.Errors),
            NotFoundException notFoundException => (404, "Recurso no encontrado", notFoundException.Message, null),
            ConflictException conflictException => (409, "Conflicto", conflictException.Message, null),
            ConcurrencyException concurrencyException => (409, "Conflicto de concurrencia", concurrencyException.Message, null),
            _ => (500, "Error interno", "Ocurrió un error inesperado.", null),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            LogUnhandledException(logger, httpContext.TraceIdentifier, exception);
        }
        else
        {
            LogHandledException(logger, status, httpContext.TraceIdentifier, exception.Message, null);
        }

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        problemDetails.Extensions["traceId"] = httpContext.Response.Headers["x-correlation-id"].FirstOrDefault()
            ?? httpContext.TraceIdentifier;

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: ProblemJsonContentType,
            cancellationToken: cancellationToken);

        return true;
    }
}
