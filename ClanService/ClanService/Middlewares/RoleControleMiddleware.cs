using System.Security.Claims;

namespace ClanService.Middlewares;

public class RoleControleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleControleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

   public async Task InvokeAsync(HttpContext context)
    {
        // 1. API Gateway'in gönderdiği UserId'yi al
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();

        // Gateway bu ID'yi yolladıysa, adam giriş yapmış demektir!
        if (!string.IsNullOrEmpty(userId))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId) 
            };

            var role = context.Request.Headers["X-Clan-Role"].FirstOrDefault();
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "GatewayAuth");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }

}
