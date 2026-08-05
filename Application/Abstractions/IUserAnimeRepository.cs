namespace Application.Abstractions;

public interface IUserAnimeRepository
{
    Task<Domain.Entities.UserAnime?> GetByUserAndAnimeAsync(Guid userId, Guid animeId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.UserAnime>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.UserAnime>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.UserAnime userAnime, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.UserAnime userAnime, CancellationToken cancellationToken = default);
}
