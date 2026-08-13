namespace Application.Abstractions;

public interface IUserAnimeRepository
{
    Task<Domain.Entities.UserAnime?> GetByUserAndAnimeAsync(Guid userId, Guid animeId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.UserAnime>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.UserAnime>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default);
}