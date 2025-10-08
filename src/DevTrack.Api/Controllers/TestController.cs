using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace DevTrack.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public ActionResult<string> PublicEndpoint()
    {
        return Ok("This is a public endpoint");
    }

    [Authorize]
    [HttpGet("protected")]
    public ActionResult<object> ProtectedEndpoint()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            Message = "This is a protected endpoint",
            UserId = userId,
            Email = email,
            Name = name,
            Roles = roles
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public ActionResult<string> AdminEndpoint()
    {
        return Ok("This is an admin-only endpoint");
    }
}