using PlatformService.Data.Interfaces;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly ApplicationDbContext _context;

    public PlatformRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Platform GetPlatformById(int id)
    {
        return _context.Platforms.FirstOrDefault(x => x.Id == id);
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null) 
            throw new ArgumentNullException(nameof(platform));

        _context.Platforms.Add(platform);
    }
}