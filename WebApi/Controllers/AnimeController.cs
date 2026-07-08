using Application.Animes;
using Application.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/anime")]
public class AnimeController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AnimeResponse>> CreateAsync(
        CreateAnimeCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Created($"/api/anime/{response.Id}", response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AnimeResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAnimeQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<AnimeResponse>>> GetAllAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? genre = null,
        [FromQuery] AnimeStatus? status = null,
        [FromQuery] string? sortBy = "created",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(
            new GetAllAnimesQuery(page, pageSize, search, genre, status, sortBy, sortOrder),
            cancellationToken);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateAnimeCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{animeId:guid}/rate")]
    public async Task<IActionResult> RateAsync(
        Guid animeId,
        RateAnimeCommand command,
        CancellationToken cancellationToken)
    {
        if (animeId != command.AnimeId)
            return BadRequest("Route AnimeId does not match command AnimeId.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAnimeCommand(id), cancellationToken);
        return NoContent();
    }
}
