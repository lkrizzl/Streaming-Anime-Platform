using Application.Auth;
using Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("users/me")]
[EnableRateLimiting("ApiPolicy")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<MeUserResponse>> GetMyProfileAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MeCommand(), cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMyProfileAsync(
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
