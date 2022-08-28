using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data.Interfaces;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformController(
        IPlatformRepository repository, 
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformRead>> GetPlatforms()
    {
        var platformItems = _repository.GetAllPlatforms();
        
        return Ok(_mapper.Map<IEnumerable<PlatformRead>>(platformItems));
    }

    [HttpGet("{id:int}", Name = nameof(GetPlatformById))]
    public ActionResult<PlatformRead> GetPlatformById(int id)
    {
        var platform = _repository.GetPlatformById(id);
        
        if (platform is null) return NotFound();
        return Ok(_mapper.Map<PlatformRead>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformRead>> CreatePlatform(PlatformCreate platformCreate)
    {
        var platformModel = _mapper.Map<Platform>(platformCreate);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();
        
        var platformRead = _mapper.Map<PlatformRead>(platformModel);
        // Send Synchronous Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformRead);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }
        
        // Send Asynchronous Message
        try
        {
            var platformPublish = _mapper.Map<PlatformPublished>(platformRead);
            platformPublish.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublish);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
        }
        
        return CreatedAtRoute(nameof(GetPlatformById), 
            new { Id = platformRead.Id }, 
            platformRead);
    }
}