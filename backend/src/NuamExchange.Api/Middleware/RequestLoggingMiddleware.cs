using System.Diagnostics;

namespace NuamExchange.Api.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private static readonly Action<ILogger, string, string, Exception?> LogRequestStarted =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(2001, "RequestStarted"),
            "HTTP {Method} {Path} started");

    private static readonly Action<ILogger, string, string, int, long, Exception?> LogRequestCompleted =
        LoggerMessage.Define<string, string, int, long>(
            LogLevel.Information,
            new EventId(2002, "RequestCompleted"),
            "HTTP {Method} {Path} responded {Status} in {Elapsed} ms");

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = httpContext.Request.Method;
        var path = httpContext.Request.Path.ToString();

        LogRequestStarted(logger, method, path, null);

        try
        {
            await next(httpContext);
        }
        finally
        {
            stopwatch.Stop();
            LogRequestCompleted(logger, method, path, httpContext.Response.StatusCode, stopwatch.ElapsedMilliseconds, null);
        }
    }
}
