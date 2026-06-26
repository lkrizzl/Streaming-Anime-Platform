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
            g => g.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Genres
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
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
