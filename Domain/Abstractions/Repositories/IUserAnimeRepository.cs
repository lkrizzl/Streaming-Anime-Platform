using Domain.Associations;

namespace Domain.Abstractions.Repositories;

public interface IUserAnimeRepository
{
    Task<UserAnime?> GetByUserAndAnimeAsync(Guid userId, Guid animeId, CancellationToken ct = default);

    Task AddAsync(UserAnime userAnime, CancellationToken ct = default);
    void Update(UserAnime userAnime);
    Task RemoveAsync(Guid userId, Guid animeId, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid userId, Guid animeId, CancellationToken ct = default);

    Task<IReadOnlyList<UserAnime>> GetUserListAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserAnime>> GetUserListByStatusAsync(Guid userId, WatchStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<UserAnime>> GetFavoritesAsync(Guid userId, CancellationToken ct = default);
}