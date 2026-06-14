using Domain.Entities;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Season>> GetSeasonsByAnimeIdAsync(Guid animeId, CancellationToken ct = default);
    Task AddAsync(Season season, CancellationToken ct = default);
}