using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SeasonRepository(AppDbContext dbContext) : ISeasonRepository
{
    public async Task<Season?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Seasons.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Season>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Seasons
            .Where(s => s.AnimeId == animeId && s.IsActive)
            .OrderBy(s => s.SeasonNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Season season, CancellationToken cancellationToken = default)
    {
        await dbContext.Seasons.AddAsync(season, cancellationToken);
    }

    public Task DeleteAsync(Season season, CancellationToken cancellationToken = default)
    {
        dbContext.Seasons.Remove(season);
        return Task.CompletedTask;
    }
}
