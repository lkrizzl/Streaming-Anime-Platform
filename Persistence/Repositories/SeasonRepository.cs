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
            .AsNoTracking()
            .Where(s => s.AnimeId == animeId && s.IsActive)
            .OrderBy(s => s.SeasonNumber.Value)
            .ToListAsync(cancellationToken);
    }
}