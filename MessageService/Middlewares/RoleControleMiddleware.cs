using System.Security.Claims;

namespace MessageService.Middlewares;

public class RoleControleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleControleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Clan-Role", out var roleValues))
        {
            var role = roleValues.FirstOrDefault();
            
            if (!string.IsNullOrEmpty(role) && context.User.Identity?.IsAuthenticated == true)
            {
                var claims = new[] { new Claim(ClaimTypes.Role, role) };
                var identity = new ClaimsIdentity(claims, "ClanRole");
                context.User.AddIdentity(identity);
            }
        }
        await _next(context);
    }

}
