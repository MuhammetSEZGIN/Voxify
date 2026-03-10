using MessageService.Hubs;
using MessageService.RabbitMq;
using MessageService.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MessageService.Interfaces.Services;
using MessageService.Extensions;

var builder = WebApplication.CreateBuilder(args);


// RabbitMQ 
builder.Services.AddRabbitMQServices(builder.Configuration);  
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// MongoDB ve repository
builder.Services.AddMongoDbService(builder.Configuration);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMessageService, MessageService.Services.MessageService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(
    provider=> new BackgroundTaskQueue(capacity:100)
);
builder.Services.AddHostedService<QueuedHostedService>();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Rate Limiting
builder.Services.AddCustomRateLimiter();
// Cors
builder.Services.AddCustomCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAll");

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message
            })
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
});
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapHub<MessageHub>("/messagehub");
app.MapHub<VoicePresenceHub>("/hubs/voice");
app.MapControllers();
app.Run();
