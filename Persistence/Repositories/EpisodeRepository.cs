using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class EpisodeRepository(AppDbContext dbContext) : IEpisodeRepository
{
    public async Task<Episode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Episodes.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Episode>> GetBySeasonIdAsync(Guid seasonId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Episodes
            .Where(e => e.SeasonId == seasonId && e.IsActive)
            .OrderBy(e => e.EpisodeNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Episode episode, CancellationToken cancellationToken = default)
    {
        await dbContext.Episodes.AddAsync(episode, cancellationToken);
    }

    public Task DeleteAsync(Episode episode, CancellationToken cancellationToken = default)
    {
        dbContext.Episodes.Remove(episode);
        return Task.CompletedTask;
    }
}
