using Application.Abstractions;
using Domain.Associations;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserAnimeRepository(AppDbContext dbContext) : IUserAnimeRepository
{
    public async Task<UserAnime?> GetByUserAndAnimeAsync(Guid userId, Guid animeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserAnimes
            .Include(ua => ua.Anime)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AnimeId == animeId, cancellationToken);
    }

    public async Task<List<UserAnime>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserAnimes
            .Include(ua => ua.Anime)
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.LastUpdatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserAnime>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserAnimes
            .Where(ua => ua.AnimeId == animeId && ua.UserRating != null)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserAnime userAnime, CancellationToken cancellationToken = default)
    {
        await dbContext.UserAnimes.AddAsync(userAnime, cancellationToken);
    }

    public Task DeleteAsync(UserAnime userAnime, CancellationToken cancellationToken = default)
    {
        dbContext.UserAnimes.Remove(userAnime);
        return Task.CompletedTask;
    }
}
