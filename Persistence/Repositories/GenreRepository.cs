using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class GenreRepository(AppDbContext dbContext) : IGenreRepository
{
    public async Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.Genres.FirstOrDefaultAsync(
            g => EF.Functions.ILike(g.Name, name), cancellationToken);
    }

    public async Task<PaginatedList<Genre>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Genres
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Genre>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        await dbContext.Genres.AddAsync(genre, cancellationToken);
    }

    public Task DeleteAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        dbContext.Genres.Remove(genre);
        return Task.CompletedTask;
    }
}
