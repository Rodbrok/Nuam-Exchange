using System.Diagnostics;
namespace NuamExchange.Api.Middleware;
public sealed class RequestLoggingMiddleware(RequestDelegate next,ILogger<RequestLoggingMiddleware> logger){public async Task InvokeAsync(HttpContext ctx){var sw=Stopwatch.StartNew();logger.LogInformation("HTTP {Method} {Path} started",ctx.Request.Method,ctx.Request.Path);await next(ctx);sw.Stop();logger.LogInformation("HTTP {Method} {Path} responded {Status} in {Elapsed} ms",ctx.Request.Method,ctx.Request.Path,ctx.Response.StatusCode,sw.ElapsedMilliseconds);}}
