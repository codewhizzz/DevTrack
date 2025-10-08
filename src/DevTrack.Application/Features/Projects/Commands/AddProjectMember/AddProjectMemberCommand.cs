using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Projects.Commands.AddProjectMember;

public record AddProjectMemberCommand : IRequest
{
    public Guid ProjectId { get; init; }
    public Guid UserId { get; init; }
    public ProjectRole Role { get; init; }
}

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddProjectMemberCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithMembersAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Check if current user is a maintainer
        if (!project.HasRole(_currentUser.UserId, ProjectRole.Maintainer))
            throw new UnauthorizedAccessException("Only project maintainers can add members");

        // Check if user exists
        var userExists = await _userRepository.GetByIdAsync(request.UserId, cancellationToken) != null;
        if (!userExists)
            throw new InvalidOperationException("User not found");

        project.AddMember(request.UserId, request.Role);
        _projectRepository.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}