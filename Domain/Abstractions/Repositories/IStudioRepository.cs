using Domain.Entities;

public interface IStudioRepository
{
    Task<Studio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Studio?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<Studio>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Studio studio, CancellationToken ct = default);
}