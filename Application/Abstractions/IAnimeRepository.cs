using Domain.Entities;

namespace Application.Abstractions;

public interface IAnimeRepository
{
    Task<Anime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Anime>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<Anime>> GetAllAsync(int page, int pageSize, AnimeFilter filter, CancellationToken cancellationToken = default);
    Task AddAsync(Anime anime, CancellationToken cancellationToken = default);
    Task UpdateAsync(Anime anime, CancellationToken cancellationToken = default);
    Task DeleteAsync(Anime anime, CancellationToken cancellationToken = default);
}
