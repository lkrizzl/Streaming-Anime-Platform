using Domain.Entities;

namespace Application.Abstractions;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Genre genre, CancellationToken cancellationToken = default);
    Task DeleteAsync(Genre genre, CancellationToken cancellationToken = default);
}
