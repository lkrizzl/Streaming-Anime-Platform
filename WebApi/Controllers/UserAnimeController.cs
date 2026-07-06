using Application.UserAnime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
public class UserAnimeController(IMediator mediator) : ControllerBase
{
    [HttpPost("anime/{animeId:guid}/watchlist")]
    public async Task<IActionResult> AddToWatchlistAsync(
        Guid animeId,
        AddToWatchlistCommand command,
        CancellationToken cancellationToken)
    {
        if (animeId != command.AnimeId)
            return BadRequest("Route AnimeId does not match command AnimeId.");

        await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMyWatchlistAsync), null);
    }

    [HttpPut("anime/{animeId:guid}/watchlist")]
    public async Task<IActionResult> UpdateWatchlistAsync(
        Guid animeId,
        UpdateWatchlistCommand command,
        CancellationToken cancellationToken)
    {
        if (animeId != command.AnimeId)
            return BadRequest("Route AnimeId does not match command AnimeId.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("anime/{animeId:guid}/watchlist")]
    public async Task<IActionResult> RemoveFromWatchlistAsync(
        Guid animeId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveFromWatchlistCommand(animeId), cancellationToken);
        return NoContent();
    }

    [HttpGet("users/me/watchlist")]
    public async Task<ActionResult<IReadOnlyList<WatchlistItemResponse>>> GetMyWatchlistAsync(
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetMyWatchlistQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("anime/{animeId:guid}/favorite")]
    public async Task<IActionResult> ToggleFavoriteAsync(
        Guid animeId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ToggleFavoriteCommand(animeId), cancellationToken);
        return NoContent();
    }

    [HttpGet("users/me/favorites")]
    public async Task<ActionResult<IReadOnlyList<WatchlistItemResponse>>> GetMyFavoritesAsync(
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetMyFavoritesQuery(), cancellationToken);
        return Ok(response);
    }
}
