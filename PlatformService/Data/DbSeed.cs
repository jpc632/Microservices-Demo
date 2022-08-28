using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class DbSeed
{
    public static void PopulateDb(this IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<ApplicationDbContext>(), isProduction);
    }

    private static void SeedData(ApplicationDbContext context, bool isProduction)
    {
        if (isProduction)
        {
            Console.WriteLine("--> Applying Migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not run migrations: {e.Message}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding Data...");
            
            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"},
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free"},
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"}
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> Data already seeded.");
        }
    }
}