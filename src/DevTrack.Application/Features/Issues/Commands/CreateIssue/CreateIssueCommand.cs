using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Issues.Commands.CreateIssue;

public record CreateIssueCommand : IRequest<CreateIssueResult>
{
    public Guid ProjectId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public IssueType Type { get; init; }
    public IssuePriority Priority { get; init; }
    public Guid? AssigneeId { get; init; }
}

public record CreateIssueResult(Guid Id, string Key);

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, CreateIssueResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateIssueCommandHandler(
        IProjectRepository projectRepository,
        IIssueRepository issueRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _issueRepository = issueRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CreateIssueResult> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        // Verify user has access to project
        var isMember = await _projectRepository.IsUserMemberAsync(request.ProjectId, _currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project");

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Get next issue number
        var nextNumber = await _projectRepository.GetNextIssueNumberAsync(request.ProjectId, cancellationToken);

        var issue = new Issue(
            request.ProjectId,
            project.Key,
            nextNumber,
            request.Title,
            request.Description,
            request.Priority,
            request.Type,
            _currentUser.UserId);

        if (request.AssigneeId.HasValue)
        {
            // Verify assignee is a project member
            var isAssigneeMember = await _projectRepository.IsUserMemberAsync(request.ProjectId, request.AssigneeId.Value, cancellationToken);
            if (!isAssigneeMember)
                throw new InvalidOperationException("Assignee must be a project member");

            issue.AssignTo(request.AssigneeId.Value);
        }

        await _issueRepository.AddAsync(issue, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateIssueResult(issue.Id, issue.Key);
    }
}