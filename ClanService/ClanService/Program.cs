using ClanService.Data;
using ClanService.Extensions;
using ClanService.RabbitMq;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddRabbitMQServices(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseUserHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // more detailed error explanation
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        c=> c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClanService API V1")
    );
}

app.UseHttpsRedirection();


app.MapControllers();

app.MigrateDatabase();
app.Run();
