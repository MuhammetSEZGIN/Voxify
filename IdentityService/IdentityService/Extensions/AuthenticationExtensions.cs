using System.Text;
using Microsoft.IdentityModel.Tokens;


// we use this in apigateway. So we dont use it in IdentityService.
// but i want it to be here for future use. 
namespace IdentityService.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer(
                    "JwtBearer",
                    jwtBearerOptions =>
                    {
                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)
                            ),
                            ValidateIssuer = true,
                            ValidIssuer = configuration["JWT:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = configuration["JWT:Audience"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromMinutes(5),
                        };
                    }
                );

            return services;
        }
    }
}
