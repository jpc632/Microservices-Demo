using CommandService.Data.Interfaces;
using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly ApplicationDbContext _context;

    public CommandRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool ExternalPlatformExists(int externalId)
    {
        return _context.Platforms.Any(p => p.ExternalId == externalId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _context.Platforms.Add(platform);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name)
            .ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }
}