using ClanService.Data;
using ClanService.Mapping;
using ClanService.Services;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClanService.RabbitMq;
using ClanService.Interfaces.Repositories;
using ClanService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IClanService, ClanService.Services.ClanService>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddScoped<IClanMembershipService, ClanMembershipService>();
builder.Services.AddScoped<IVoiceChannelService, VoiceChannelService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<IClanRepository, ClanRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClanMembershipRepository, ClanMembershipRepository>();
builder.Services.AddScoped<IClanInvitation, ClanInvitationRepository>();
builder.Services.AddScoped<IVoiceChannelRepository, VoiceChannelRepository>();

builder.Services.AddRabbitMQServices(builder.Configuration);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors("AllowAll");
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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();

using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;
try
{
    var db = service.GetRequiredService<ApplicationDbContext>();
    db.Database.CanConnect();
    db.Database.EnsureCreated();
    db.Database.Migrate();
    var logger = service.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Database Migrated");
}
catch
{
    var logger = service.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("An error occured while migrating the database");
}
app.Run();
