using DevTrack.Application.Features.Issues.Commands.CreateIssue;
using DevTrack.Domain.Entities; // Add this line
using MediatR;
using DevTrack.Application.Features.Issues.Queries.GetProjectIssues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DevTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/issues")]
public class IssuesController : ControllerBase
{
    private readonly ISender _mediator;

    public IssuesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateIssueResult>> CreateIssue(Guid projectId, [FromBody] CreateIssueRequest request)
    {
        var command = new CreateIssueCommand
        {
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetIssue), new { projectId, issueId = result.Id }, result);
    }

    [HttpGet]
public async Task<ActionResult<PagedResult<IssueListDto>>> GetProjectIssues(
    Guid projectId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] IssueStatus? status = null,
    [FromQuery] Guid? assigneeId = null,
    [FromQuery] string sortBy = "CreatedAt",
    [FromQuery] bool sortDescending = true)
{
    var query = new GetProjectIssuesQuery
    {
        ProjectId = projectId,
        PageNumber = pageNumber,
        PageSize = pageSize,
        Status = status,
        AssigneeId = assigneeId,
        SortBy = sortBy,
        SortDescending = sortDescending
    };

    var result = await _mediator.Send(query);
    return Ok(result);
}
}

public record CreateIssueRequest(
    string Title,
    string Description,
    IssueType Type,
    IssuePriority Priority,
    Guid? AssigneeId);