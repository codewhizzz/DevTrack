using DevTrack.Application.Features.Users.Commands.RegisterUser;
using DevTrack.Application.Features.Users.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevTrack.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResult>> Register(RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResult>> Login(LoginUserQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}