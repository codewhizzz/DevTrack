using DevTrack.Application.Features.Projects.Commands.AddProjectMember;
using DevTrack.Application.Features.Projects.Commands.CreateProject;
using DevTrack.Application.Features.Projects.Queries.GetProjectById;
using DevTrack.Domain.Entities; // Add this line
using MediatR;
using DevTrack.Application.Features.Projects.Queries.GetUserProjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DevTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ISender _mediator;

    public ProjectsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateProjectResult>> CreateProject(CreateProjectCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
public async Task<ActionResult<List<UserProjectDto>>> GetMyProjects()
{
    var result = await _mediator.Send(new GetUserProjectsQuery());
    return Ok(result);
}

    [HttpPost("{projectId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid projectId, [FromBody] AddMemberRequest request)
    {
        await _mediator.Send(new AddProjectMemberCommand
        {
            ProjectId = projectId,
            UserId = request.UserId,
            Role = request.Role
        });
        return NoContent();
    }
}

public record AddMemberRequest(Guid UserId, ProjectRole Role);