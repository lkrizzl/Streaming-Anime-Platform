using Application.Episodes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class EpisodeController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("api/seasons/{seasonId:guid}/episodes")]
    public async Task<ActionResult<EpisodeResponse>> CreateAsync(
        Guid seasonId,
        CreateEpisodeCommand command,
        CancellationToken cancellationToken)
    {
        if (seasonId != command.SeasonId)
            return BadRequest("Route SeasonId does not match command SeasonId.");

        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpGet("api/episodes/{id:guid}")]
    public async Task<ActionResult<EpisodeResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetEpisodeQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet("api/seasons/{seasonId:guid}/episodes")]
    public async Task<ActionResult<IReadOnlyList<EpisodeResponse>>> GetBySeasonAsync(
        Guid seasonId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetEpisodesBySeasonQuery(seasonId), cancellationToken);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/episodes/{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateEpisodeCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/episodes/{id:guid}/publish")]
    public async Task<IActionResult> PublishAsync(
        Guid id,
        PublishEpisodeCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/episodes/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteEpisodeCommand(id), cancellationToken);
        return NoContent();
    }
}
