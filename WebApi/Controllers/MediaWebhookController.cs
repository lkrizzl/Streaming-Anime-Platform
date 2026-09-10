using Application.Episodes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public record MediaReadyRequest(string StreamUrl);

[ApiController]
[AllowAnonymous]
public class MediaWebhookController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [HttpPost("api/internal/episodes/{id:guid}/media-ready")]
    public async Task<IActionResult> MediaReadyAsync(
        Guid id,
        MediaReadyRequest request,
        CancellationToken cancellationToken)
    {
        var expectedKey = configuration["MediaService:InternalApiKey"];

        if (string.IsNullOrEmpty(expectedKey) ||
            !Request.Headers.TryGetValue("X-Internal-Api-Key", out var providedKey) ||
            providedKey != expectedKey)
        {
            return Unauthorized();
        }

        await mediator.Send(new SetEpisodeVideoUrlCommand(id, new Uri(request.StreamUrl)), cancellationToken);

        return NoContent();
    }
}