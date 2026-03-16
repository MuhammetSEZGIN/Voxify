using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Docker"))
{
    builder.Configuration.AddJsonFile("ocelot.docker.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

var corsConfigOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var corsDefaultOrigins = new[] { "http://localhost:5173", "tauri://localhost", "https://tauri.localhost","http://tauri.localhost" };
var corsAllowedOrigins = corsConfigOrigins.Union(corsDefaultOrigins).ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTauri", policy =>
    {
        policy.WithOrigins(corsAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // token ın güvenlir bir anahtarla imzalanıp imzalanmadığını kontrol eder
            ValidateIssuerSigningKey = true,
            // token imzansını doğrulamak için kullanılan anahtar
            // bu anahtar, JWT oluşturulurken kullanılan anahtarla aynı olmalıdır
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(
                    builder.Configuration["JWT:Key"]!
                )
            ),

            /*
                Token'ı kimin oluşturduğunu (Issuer) ve kimin için oluşturulduğunu (Audience) doğrulama adımlarını devre dışı bırakır.
                Genellikle API Gateway senaryolarında bu kontroller aşağı akış servislerinde yapılır.
            */
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            //Token'ın süresinin dolup dolmadığını kontrol eder.
            ValidateLifetime = true,
            // Token süresi kontrolünde sunucular arasındaki saat farklarına tolerans tanır
            // (burada sıfır olarak ayarlanmış, yani tolerans yok).
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowTauri");

// Gelen isteklerden sahte X-User-* header'larını temizle (header injection koruması)
app.Use(async (context, next) =>
{
    context.Request.Headers.Remove("X-User-Id");
    context.Request.Headers.Remove("X-User-Name");
    context.Request.Headers.Remove("X-User-Avatar");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(
    async (context, next) =>
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Request.Headers.Append(
                "X-User-Id",
                context.User.FindFirst("sub")?.Value ?? string.Empty
            );
            context.Request.Headers.Append(
                "X-User-Name",
                context.User.FindFirst("unique_name")?.Value ?? string.Empty
            );
            context.Request.Headers.Append(
                "X-User-Avatar",
                context.User.FindFirst("picture")?.Value ?? string.Empty
            );
        }
        await next();
    }
);

await app.UseOcelot();
app.Run();
