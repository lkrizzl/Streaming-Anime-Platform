using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AnimeRepository(AppDbContext dbContext) : IAnimeRepository
{
    public async Task<Anime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Anime
            .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .Include(a => a.AnimeStudios)
                .ThenInclude(ast => ast.Studio)
            .Include(a => a.Seasons.OrderBy(s => s.SeasonNumber))
                .ThenInclude(s => s.Episodes.OrderBy(e => e.EpisodeNumber))
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Anime>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Anime
            .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .Include(a => a.AnimeStudios)
                .ThenInclude(ast => ast.Studio)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        await dbContext.Anime.AddAsync(anime, cancellationToken);
    }

    public Task UpdateAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        dbContext.Anime.Update(anime);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Anime anime, CancellationToken cancellationToken = default)
    {
        dbContext.Anime.Remove(anime);
        return Task.CompletedTask;
    }
}
