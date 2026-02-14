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

builder.Services.AddCors();
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
                    builder.Configuration["JWT:Key"] ?? "YourTemporaryKeyHere12345678901234567890"
                )
            ),

            /*
                Token'ı kimin oluşturduğunu (Issuer) ve kimin için oluşturulduğunu (Audience) doğrulama adımlarını devre dışı bırakır.
                Genellikle API Gateway senaryolarında bu kontroller aşağı akış servislerinde yapılır.
            */
            ValidateIssuer = false,
            ValidateAudience = false,
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

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

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
