using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Platform, PlatformRead>();
        CreateMap<Command, CommandRead>();
        CreateMap<CommandCreate, Command>();
        CreateMap<PlatformPublish, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                options => options
                    .MapFrom(src => src.Id));
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                options => options
                    .MapFrom(src => src.PlatformId))
            .ForMember(
                dest => dest.Commands, 
                options => options.Ignore());
    }
}