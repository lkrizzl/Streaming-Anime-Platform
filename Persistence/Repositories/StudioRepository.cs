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
            s => s.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Studio>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Studios
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
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
