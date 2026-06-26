using Domain.Entities;

namespace Application.Abstractions;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Season>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default);
    Task AddAsync(Season season, CancellationToken cancellationToken = default);
    Task DeleteAsync(Season season, CancellationToken cancellationToken = default);
}
