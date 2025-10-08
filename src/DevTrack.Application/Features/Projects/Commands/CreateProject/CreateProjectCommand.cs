using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<CreateProjectResult>
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string Key { get; init; }
}

public record CreateProjectResult(Guid Id, string Key);

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Check if project key already exists
        var keyExists = await _projectRepository.ProjectKeyExistsAsync(request.Key, cancellationToken);
        if (keyExists)
            throw new InvalidOperationException($"Project key '{request.Key}' already exists");

        // Create project with current user as maintainer
        var project = new Project(
            request.Name,
            request.Description,
            request.Key,
            _currentUser.UserId);

        await _projectRepository.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProjectResult(project.Id, project.Key);
    }
}