using IdentityService.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            return services;
        }

        public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;

            try
            {
                var db = service.GetRequiredService<IdentityDbContext>();

                if (db.Database.CanConnect())
                {
                    await db.Database.MigrateAsync();
                    var logger = service.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Database Migrated Successfully");
                }
                else
                {
                    var logger = service.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Cannot connect to database");
                }
            }
            catch
            {
                var logger = service.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("An error occurred while migrating the database");
            }

            return app;
        }
    }
}
