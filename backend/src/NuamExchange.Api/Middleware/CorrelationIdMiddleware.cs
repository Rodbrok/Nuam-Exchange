namespace NuamExchange.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string Header = "x-correlation-id";

    public async Task InvokeAsync(HttpContext httpContext, ILogger<CorrelationIdMiddleware> logger)
    {
        var incoming = httpContext.Request.Headers[Header].FirstOrDefault();
        var correlationId = IsSafe(incoming) ? incoming! : Guid.NewGuid().ToString("N");

        httpContext.TraceIdentifier = correlationId;
        httpContext.Response.Headers[Header] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } }))
        {
            await next(httpContext);
        }
    }

    private static bool IsSafe(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && value.Length <= 64
        && value.All(character => char.IsLetterOrDigit(character) || character is '-' or '_');
}
