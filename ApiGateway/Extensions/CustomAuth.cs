using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace ApiGateway.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorization(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
           
            
            return services;
        }
    }
}
