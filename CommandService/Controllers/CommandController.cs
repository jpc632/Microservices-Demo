using AutoMapper;
using CommandService.Data.Interfaces;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/platform/{platformId:int}/[controller]")]
[ApiController]
public class CommandController : ControllerBase
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public CommandController(ICommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandRead>> GetCommandsForPlatform(int platformId)
    {
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var commandItems = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandRead>>(commandItems));
    }

    [HttpGet("{commandId:int}", Name = nameof(GetCommandForPlatform))]
    public ActionResult<Command> GetCommandForPlatform(int platformId, int commandId)
    {
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var commandItem = _repository.GetCommand(platformId, commandId);
        if (commandItem is null) return NotFound();
        
        return Ok(_mapper.Map<CommandRead>(commandItem));
    }

    [HttpPost]
    public ActionResult<CommandRead> CreateCommand(int platformId, CommandCreate commandCreate)
    {
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var commandItem = _mapper.Map<Command>(commandCreate);
        _repository.CreateCommand(platformId, commandItem);
        _repository.SaveChanges();

        var commandRead = _mapper.Map<CommandRead>(commandItem);

        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new { platformId, commandId = commandRead.Id },
            commandRead);
    }
}