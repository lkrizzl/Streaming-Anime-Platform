using Domain.Entities;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Genre?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Genre genre, CancellationToken ct = default);
}