using System;

namespace ClanService.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(
            options =>
            {
                options.AddPolicy("AllowAll", policy =>
            {
                policy.SetIsOriginAllowed(origin=>true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
            });
            }
        );
        return services;
    }
}
