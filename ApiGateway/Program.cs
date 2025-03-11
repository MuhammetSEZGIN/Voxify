using Ocelot.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsEnvironment("Docker"))
{
    builder.Configuration.AddJsonFile("ocelot.docker.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

builder.Configuration.AddJsonFile("swagger.json", optional: false, reloadOnChange: true);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["JWT:Key"] ?? "YourTemporaryKeyHere12345678901234567890")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
    });
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        context.Request.Headers.Append("X-User-Id",
            context.User.FindFirst("sub")?.Value ?? string.Empty);
        context.Request.Headers.Append("X-User-Name",
            context.User.FindFirst("unique_name")?.Value ?? string.Empty);
        context.Request.Headers.Append("X-User-Avatar",
            context.User.FindFirst("picture")?.Value ?? string.Empty);
    }
    await next();
});

await app.UseOcelot();
app.Run();
