using CommandService.Data.Interfaces;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;

public static class DbSeed
{
    public static void PopulateDb(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        IEnumerable<Platform> platforms = grpcClient.ReturnAllPlatforms();

        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        foreach (var platform in platforms)
        {
            if (!repository.ExternalPlatformExists(platform.ExternalId))
            {
                repository.CreatePlatform(platform);
            }

            repository.SaveChanges();
        }
    }
}