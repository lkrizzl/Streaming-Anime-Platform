using Application.Abstractions;
using Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApі.Controllers;

public record SignUpRequest(string Email, string Username, string Password, bool RememberMe);

public record SignInRequest(string UsernameOrEmail, string Password, bool RememberMe);

[ApiController]
[Route("auth")]
public class AuthController(
    IMediator mediator,
    IClaimsPrincipalProvider claimsPrincipalProvider) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> MeAsync(CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new MeCommand(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(
            new SignUpCommand(request.Email, request.Username, request.Password),
            cancellationToken);

        var principal = claimsPrincipalProvider.Create(
            response.Id, response.Username, response.Email, response.SecurityStamp);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = request.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1)
            });

        return Ok();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(
            new SignInCommand(request.UsernameOrEmail, request.Password),
            cancellationToken);

        var principal = claimsPrincipalProvider.Create(
            response.Id, response.Username, response.Email, response.SecurityStamp);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = request.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1)
            });

        return Ok();
    }

    [Authorize]
    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync(CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [Authorize]
    [HttpPost("sign-out-all")]
    public async Task<IActionResult> SignOutAllAsync(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new SignOutAllCommand(), cancellationToken);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}
