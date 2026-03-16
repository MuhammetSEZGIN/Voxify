using ClanService.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClanService.Extensions;

public static class DatabaseExtensions
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();

            // EnsureCreated and Migrate must not be used together.
            // In production we rely only on migrations so EF's history table stays consistent.
            db.Database.Migrate();
            logger.LogInformation("Database Migrated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while migrating the database. Application cannot start.");
            throw;
        }

        return app;
    }
}
