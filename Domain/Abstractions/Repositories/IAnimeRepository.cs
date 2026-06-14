using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IAnimeRepository
{
    Task<Anime?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Anime?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default); // жанри, студії, сезони

    Task<IReadOnlyList<Anime>> GetPopularAsync(int take = 10, CancellationToken ct = default);
    Task<IReadOnlyList<Anime>> GetByStatusAsync(AnimeStatus status, int take = 20, CancellationToken ct = default);

    Task AddAsync(Anime anime, CancellationToken ct = default);
    void Update(Anime anime);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}