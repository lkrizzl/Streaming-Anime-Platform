using Domain.Entities;

namespace Application.Abstractions;

public interface IAnimeRepository
{
    Task<Anime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Anime>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Anime anime, CancellationToken cancellationToken = default);
    Task UpdateAsync(Anime anime, CancellationToken cancellationToken = default);
    Task DeleteAsync(Anime anime, CancellationToken cancellationToken = default);
}
