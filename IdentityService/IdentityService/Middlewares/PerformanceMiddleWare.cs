using System;

namespace IdentityService.Middlewares;

public class PerformanceMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleWare> _logger;

    public PerformanceMiddleWare(RequestDelegate next, ILogger<PerformanceMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await _next(context);
        watch.Stop();
        _logger.LogInformation(
            "Request {Method} {Path} took {ElapsedMilliseconds}ms - Status: {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            watch.ElapsedMilliseconds,
            context.Response.StatusCode
        );
    }
}
