using Application.Seasons;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class SeasonController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("api/anime/{animeId:guid}/seasons")]
    public async Task<ActionResult<SeasonResponse>> CreateAsync(
        Guid animeId,
        CreateSeasonCommand command,
        CancellationToken cancellationToken)
    {
        if (animeId != command.AnimeId)
            return BadRequest("Route AnimeId does not match command AnimeId.");

        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpGet("api/seasons/{id:guid}")]
    public async Task<ActionResult<SeasonResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetSeasonQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet("api/anime/{animeId:guid}/seasons")]
    public async Task<ActionResult<IReadOnlyList<SeasonResponse>>> GetByAnimeAsync(
        Guid animeId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetSeasonsByAnimeQuery(animeId), cancellationToken);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/seasons/{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateSeasonCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/seasons/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSeasonCommand(id), cancellationToken);
        return NoContent();
    }
}
