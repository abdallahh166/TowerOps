namespace TowerOps.Infrastructure.Persistence.SeedData;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Infrastructure.Persistence;

public static class DatabaseSeedExtensions
{
    public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            var settingsEncryptionService = services.GetRequiredService<ISettingsEncryptionService>();
            
            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed data
            var seeder = new DatabaseSeeder(context, logger, settingsEncryptionService);
            await seeder.SeedAsync();
        }
        catch (PlatformNotSupportedException ex)
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogWarning(ex,
                "Database seeding skipped: platform does not support configured provider/connection. " +
                "If using SQL Server LocalDB, switch DefaultConnection to a server-based SQL Server instance.");
        }
        catch (SqlException ex)
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogWarning(ex,
                "Database seeding skipped: unable to connect to SQL Server using DefaultConnection.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}

