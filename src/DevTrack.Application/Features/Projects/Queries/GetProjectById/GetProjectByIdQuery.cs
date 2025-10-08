using AutoMapper;
using DevTrack.Application.Common.Attributes;
using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Projects.Queries.GetProjectById;

[Cacheable(5)] // Cache for 5 minutes
public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;

public class ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Key { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<ProjectMemberDto> Members { get; init; }
}

public class ProjectMemberDto
{
    public Guid UserId { get; init; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public ProjectRole Role { get; init; }
    public DateTime JoinedAt { get; init; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithMembersAsync(request.Id, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Check if user has access
        if (!project.IsMember(_currentUser.UserId))
            throw new UnauthorizedAccessException("You are not a member of this project");

        var projectDto = _mapper.Map<ProjectDto>(project);

        // Enrich member data with user details
        foreach (var member in projectDto.Members)
        {
            var user = await _userRepository.GetByIdAsync(member.UserId, cancellationToken);
            if (user != null)
            {
                member.UserName = user.Name;
                member.UserEmail = user.Email;
            }
        }

        return projectDto;
    }
}