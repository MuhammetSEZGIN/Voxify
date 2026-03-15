namespace ClanService.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseUserHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                context.Items["UserId"] = userId.ToString();

                if (context.Request.Headers.TryGetValue("X-User-Name", out var userName))
                    context.Items["UserName"] = userName.ToString();
                if (context.Request.Headers.TryGetValue("X-User-Avatar", out var userAvatar))
                    context.Items["UserAvatar"] = userAvatar.ToString();
            }

            await next();
        });

        return app;
    }
}
