using Domain.Entities;

public interface IEpisodeRepository
{
    Task<Episode?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Episode>> GetEpisodesBySeasonIdAsync(Guid seasonId, CancellationToken ct = default);
    Task AddAsync(Episode episode, CancellationToken ct = default);
}