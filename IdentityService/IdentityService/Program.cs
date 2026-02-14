using IdentityService.Extensions;
using IdentityService.Messaging;
using IdentityService.Middlewares;
using Microsoft.AspNetCore.Builder;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Service configurations
builder.Services.AddApiConfiguration(); //  This already includes Swagger
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsConfiguration();
builder.Services.AddApplicationServices();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// RabbitMQ
builder.Services.AddRabbitMQProducer(builder.Configuration);

var app = builder.Build();

// Middleware configuration
app.UseMiddleware<PerformanceMiddleWare>();

// Pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API V1");
        c.RoutePrefix = string.Empty; // ✅ This should work now
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Database migration
await app.ApplyMigrationsAsync();
app.Run();

// Make Program accessible for integration tests
public partial class Program { }
