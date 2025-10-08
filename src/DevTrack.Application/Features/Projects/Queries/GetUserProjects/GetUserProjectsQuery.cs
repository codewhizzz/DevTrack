using AutoMapper;
using DevTrack.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Projects.Queries.GetUserProjects;

public record GetUserProjectsQuery : IRequest<List<UserProjectDto>>;

public record UserProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Key { get; init; }
    public string Description { get; init; }
    public string Role { get; init; }
    public int IssueCount { get; init; }
}

public class GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, List<UserProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetUserProjectsQueryHandler(
        IProjectRepository projectRepository,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<List<UserProjectDto>> Handle(GetUserProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetProjectsForUserAsync(_currentUser.UserId, cancellationToken);
        
        var projectDtos = new List<UserProjectDto>();
        
        foreach (var project in projects)
        {
            var userRole = project.Members.FirstOrDefault(m => m.UserId == _currentUser.UserId)?.Role;
            
            projectDtos.Add(new UserProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Key = project.Key,
                Description = project.Description,
                Role = userRole?.ToString() ?? "Unknown",
                IssueCount = project.Issues.Count
            });
        }

        return projectDtos.OrderBy(p => p.Name).ToList();
    }
}