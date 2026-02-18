namespace TelecomPM.Api.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Incoming {Method} request to {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        var duration = DateTime.UtcNow - startedAt;

        _logger.LogInformation(
            "Completed {Method} {Path} with status {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}

