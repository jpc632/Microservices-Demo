using CommandService.Models;

namespace CommandService.Data.Interfaces;

public interface ICommandRepository
{
    bool SaveChanges();
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
    bool ExternalPlatformExists(int externalId);
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}