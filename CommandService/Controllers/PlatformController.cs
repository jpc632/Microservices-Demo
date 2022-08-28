using AutoMapper;
using CommandService.Data.Interfaces;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public PlatformController(ICommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformRead>> GetPlatforms()
    {
        var platformItems = _repository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformRead>>(platformItems));
    }
}