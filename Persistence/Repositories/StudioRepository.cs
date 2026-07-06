using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class StudioRepository(AppDbContext dbContext) : IStudioRepository
{
    public async Task<Studio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Studios.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Studio?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.Studios.FirstOrDefaultAsync(
            s => EF.Functions.ILike(s.Name, name), cancellationToken);
    }

    public async Task<PaginatedList<Studio>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Studios
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Studio>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(Studio studio, CancellationToken cancellationToken = default)
    {
        await dbContext.Studios.AddAsync(studio, cancellationToken);
    }

    public Task DeleteAsync(Studio studio, CancellationToken cancellationToken = default)
    {
        dbContext.Studios.Remove(studio);
        return Task.CompletedTask;
    }
}
