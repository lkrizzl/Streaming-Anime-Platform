using Domain.Entities;

namespace Application.Abstractions;

public interface IStudioRepository
{
    Task<Studio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Studio?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<PaginatedList<Studio>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Studio studio, CancellationToken cancellationToken = default);
    Task DeleteAsync(Studio studio, CancellationToken cancellationToken = default);
}
