using PresenceService.Extensions;
using PresenceService.Hubs;
using PresenceService.Interfaces;
using PresenceService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPresenceRepository, PresenceRepository>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomCors();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PresenceHub>("/hubs/presence");

app.Run();

