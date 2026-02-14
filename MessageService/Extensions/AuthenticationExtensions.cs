using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MessageService.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtkey = configuration["jwt:key"];
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            /*
                            Bu ayar true olduğunda, alınan JWT'nin imzasının doğrulanması gerektiğini belirtir. 
                            İmza, token'ın gerçekten sizin tarafınızdan
                            (veya güvendiğiniz bir yayıncı tarafından) oluşturulduğunu ve yolda değiştirilmediğini garanti eder.
                            */
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey)),
                            /*
                             Token imzasını doğrulamak için kullanılacak anahtarı (jwtKey) belirtir
                            */
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken) &&
                                (context.HttpContext.WebSockets.IsWebSocketRequest ||
                                 context.Request.Headers["Accespt"] == "text/event-stream"))
                                    context.Token = accessToken;
                                return Task.CompletedTask;
                            }

                        };
                    }
            );
        return services;
    }
}
