using Domain.Entities;

namespace Application.Abstractions;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<PaginatedList<Genre>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Genre genre, CancellationToken cancellationToken = default);
    Task DeleteAsync(Genre genre, CancellationToken cancellationToken = default);
}
