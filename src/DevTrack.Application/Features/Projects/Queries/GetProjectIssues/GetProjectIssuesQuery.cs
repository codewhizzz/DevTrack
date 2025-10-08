using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Issues.Queries.GetProjectIssues;

public record GetProjectIssuesQuery : IRequest<PagedResult<IssueListDto>>
{
    public Guid ProjectId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public IssueStatus? Status { get; init; }
    public Guid? AssigneeId { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}

public record IssueListDto
{
    public Guid Id { get; init; }
    public string Key { get; init; }
    public string Title { get; init; }
    public IssueStatus Status { get; init; }
    public IssuePriority Priority { get; init; }
    public IssueType Type { get; init; }
    public string AssigneeName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record PagedResult<T>
{
    public List<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class GetProjectIssuesQueryHandler : IRequestHandler<GetProjectIssuesQuery, PagedResult<IssueListDto>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUser;

    public GetProjectIssuesQueryHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        ICurrentUserService currentUser)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<IssueListDto>> Handle(GetProjectIssuesQuery request, CancellationToken cancellationToken)
    {
        // Check access
        var isMember = await _projectRepository.IsUserMemberAsync(request.ProjectId, _currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project");

        // TODO: Implement in repository with proper pagination
        // For now, returning empty result
        return new PagedResult<IssueListDto>
        {
            Items = new List<IssueListDto>(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 0
        };
    }
}