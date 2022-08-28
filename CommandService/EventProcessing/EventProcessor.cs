using System.Text.Json;
using AutoMapper;
using CommandService.Data.Interfaces;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message); 
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event...");

        var eventType = JsonSerializer.Deserialize<GenericEvent>(notificationMessage);

        switch (eventType?.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published event detected.");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Could not determine event type.");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        var platformPublished = JsonSerializer.Deserialize<PlatformPublish>(platformPublishedMessage);

        try
        {
            var platform = _mapper.Map<Platform>(platformPublished);
            if (!repository.ExternalPlatformExists(platform.ExternalId))
            {
                repository.CreatePlatform(platform);
                repository.SaveChanges();
                Console.WriteLine("--> Created Platform!");
            }
            else
            {
                Console.WriteLine("--> Platform already exists!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not add platform to db: {e.Message}");
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined,
    }
}