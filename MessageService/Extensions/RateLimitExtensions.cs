using System;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace MessageService.Extensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddCustomRateLimiter(
        this IServiceCollection services
    )
    {
        services.AddRateLimiter(
            options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromSeconds(30);
                opt.QueueLimit = 0;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            options.OnRejected = async (context, token) =>
            {
                var httpContext = context.HttpContext;
                httpContext.Response.StatusCode = 429;
                context.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.HttpContext.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
                context.HttpContext.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
                context.HttpContext.Response.ContentType = "application/json";
                var response = new
                {
                    error = "Too many requests",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = 20 // seconds
                }; 
                await httpContext.Response.WriteAsJsonAsync(response, token);
            };
        }
        );
        return services;
    }
}
