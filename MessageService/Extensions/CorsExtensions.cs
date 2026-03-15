using System;

namespace MessageService.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var configOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var defaultOrigins = new[] { "https://voxify.com.tr", "https://www.voxify.com.tr", "http://localhost:5173", "tauri://localhost", "https://tauri.localhost" };
        var allowedOrigins = configOrigins.Union(defaultOrigins).ToArray();

        services.AddCors(
            options =>
            {
                options.AddPolicy("AllowTauri", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
                
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            }
        );
        return services;
    }
}
