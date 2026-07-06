namespace Application.Abstractions;

public interface IUserAnimeRepository
{
    Task<Domain.Associations.UserAnime?> GetByUserAndAnimeAsync(Guid userId, Guid animeId, CancellationToken cancellationToken = default);
    Task<List<Domain.Associations.UserAnime>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Domain.Associations.UserAnime>> GetByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Associations.UserAnime userAnime, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Associations.UserAnime userAnime, CancellationToken cancellationToken = default);
}
