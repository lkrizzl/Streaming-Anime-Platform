using Application.Genres;
using Application.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/genres")]
public class GenreController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GenreResponse>> CreateAsync(
        CreateGenreCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GenreResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetGenreQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<GenreResponse>>> GetAllAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new GetAllGenresQuery(page, pageSize), cancellationToken);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateGenreCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteGenreCommand(id), cancellationToken);
        return NoContent();
    }
}
