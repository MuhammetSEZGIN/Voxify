using ClanService.Data;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Extensions;

public static class DatabaseExtensions
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            db.Database.CanConnect();
            db.Database.EnsureCreated();
            db.Database.Migrate();
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Database Migrated");
        }
        catch
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("An error occured while migrating the database");
        }

        return app;
    }
}
