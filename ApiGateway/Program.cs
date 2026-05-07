using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

var authServiceBaseUrl = builder.Configuration["AuthService:BaseUrl"]
    ?? (builder.Environment.IsEnvironment("Docker")
        ? "http://authenticationservice:8081"
        : "http://localhost:8081");

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(authServiceBaseUrl);
});

// 1. Önce yazdığımız Handler'ı sisteme (DI Container) tanıtıyoruz
//builder.Services.AddTransient<ApiGateway.Handlers.ClanRoleEnrichmentHandler>();

// 2. Sonra Ocelot'a bu Handler'ı kullanmasını söylüyoruz!
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
            // 1. KULLANICI KİMLİĞİ KONTROLÜ
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? context.User.FindFirst("sub")?.Value 
                         ?? string.Empty;

            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value 
                           ?? context.User.FindFirst("unique_name")?.Value 
                           ?? string.Empty;

            context.Request.Headers.Append("X-User-Id", userId);
            context.Request.Headers.Append("X-User-Name", userName);

            // ========================================================
            // 2. KÜRESEL CLAN ROLÜ KONTROLÜ (SENİN İSTEDİĞİN YER)
            // ========================================================
            var path = context.Request.Path.Value;
            var match = Regex.Match(path ?? "", @"/clanId/([a-fA-F0-9\-]{36})", RegexOptions.IgnoreCase);

            if (match.Success && !string.IsNullOrEmpty(userId))
            {
                var clanId = match.Groups[1].Value;
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                
                // Program.cs içinden HttpClient'ı çekiyoruz
                var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
                var authClient = httpClientFactory.CreateClient("AuthService");

                // Java'ya soruyoruz
                HttpResponseMessage response;
                try
                {
                    response = await authClient.GetAsync($"/roles?userId={userId}&clanId={clanId}");
                }
                catch (HttpRequestException ex)
                {
                    logger.LogWarning(ex, "AuthService rol sorgusu basarisiz. userId={UserId}, clanId={ClanId}", userId, clanId);
                    await next();
                    return;
                }

                logger.LogInformation(response.ToString());
                
               if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        try
                        {
                            // 1. Gelen metni JSON objesine çevir
                            using var jsonDoc = JsonDocument.Parse(jsonString);
                            
                            // 2. İçinde "roles" adında bir alan var mı diye bak
                            if (jsonDoc.RootElement.TryGetProperty("roles", out var roleElement))
                            {
                                var cleanRole = roleElement.GetString();
                                
                                if (!string.IsNullOrEmpty(cleanRole))
                                {
                                    // 3. Alt servislere sadece tertemiz "OWNER" veya "ADMIN" yazısını yolla!
                                    context.Request.Headers.Append("X-Clan-Role", cleanRole.ToUpper());
                                    logger.LogInformation("Zenginleştirilen Rol: {Role}", cleanRole);
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Eğer Java'dan dönen şey geçerli bir JSON değilse (Düz metinse)
                            // Sistemin çökmesini engeller ve düz metin olarak eklemeyi dener
                            context.Request.Headers.Append("X-Clan-Role", jsonString.Trim().Trim('"').ToUpper());
                        }
                    }
                }
            }
        }
        await next();
    }
);

await app.UseOcelot();
app.Run();
