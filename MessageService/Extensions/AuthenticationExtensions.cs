using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MessageService.Extensions;

// bunu kullanmıyoruz şu anda
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

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
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                            /*
                             Token imzasını doğrulamak için kullanılacak anahtarı (jwtKey) belirtir
                            */
                            ValidateIssuer = true,
                            ValidIssuer = jwtIssuer,
                            ValidateAudience = true,
                            ValidAudience = jwtAudience,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments("/messagehub"))
                                    context.Token = accessToken;
                                return Task.CompletedTask;
                            }

                        };
                    }
            );
        return services;
    }
}
