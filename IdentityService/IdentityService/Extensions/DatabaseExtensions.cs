using IdentityService.Data;
using MassTransit.Caching;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IdentityService.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger
        )
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool isPostgreAvailable = false;
            try
            {

                using var testConnection = new NpgsqlConnection(connectionString);
                testConnection.Open();
                isPostgreAvailable = true;
                logger.LogInformation("Successfully connected to PostgreSQL.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PostgreSQL is not available. Falling back to SQLite.");
            }
            if (isPostgreAvailable)
            {
                services.AddDbContext<IdentityDbContext>(options =>
                                    options.UseNpgsql(connectionString)
                                );
                logger.LogInformation("Using PostgreSQL as the database provider.");
            }
            else
            {
                services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlite("Data Source=fallback.db"));
                logger.LogInformation("Using SQLite as the fallback database provider.");
            }
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
