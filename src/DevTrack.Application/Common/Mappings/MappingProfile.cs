using AutoMapper;
using DevTrack.Application.Features.Projects.Queries.GetProjectById;
using DevTrack.Domain.Entities;

namespace DevTrack.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectMember, ProjectMemberDto>()
            .ForMember(d => d.UserName, opt => opt.Ignore())
            .ForMember(d => d.UserEmail, opt => opt.Ignore());
    }
}