using Domain.Entities;

namespace Application.Abstractions;

public interface IEpisodeRepository
{
    Task<Episode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Episode>> GetBySeasonIdAsync(Guid seasonId, CancellationToken cancellationToken = default);
}