using PresenceService.Extensions;
using PresenceService.Hubs;
using PresenceService.Interfaces;
using PresenceService.RabbitMQ;
using PresenceService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPresenceRepository, PresenceRepository>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddRabbitMQServices(builder.Configuration);
var app = builder.Build();

app.UseRouting();
app.UseCors("AllowTauri");

// Normalize double slashes in request path before routing
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.Contains("//", StringComparison.Ordinal))
    {
        while (path.StartsWith("//", StringComparison.Ordinal))
        {
            path = path[1..];
        }
        context.Request.Path = new PathString(path);
    }
    await next();
});

app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PresenceHub>("/hubs/presence").RequireCors("AllowTauri");

app.Run();

