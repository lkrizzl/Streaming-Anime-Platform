using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Abstractions;

public record AnimeListItem(
    Guid Id,
    Description Title,
    Description OriginalTitle,
    Description? EnglishTitle,
    Synopsis Description,
    ReleaseYear ReleaseYear,
    AnimeStatus Status,
    ImageUrl? CoverImageUrl,
    ImageUrl? BannerImageUrl,
    ImageUrl? TrailerUrl,
    AgeRating AgeRating,
    Rating AverageRating,
    int RatingCount,
    int EpisodesCount,
    bool IsActive,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc,
    IReadOnlyList<string> GenreNames,
    IReadOnlyList<string> StudioNames)
{
    // Ensure empty lists instead of null for safe serialization
    public IReadOnlyList<string> GenreNames { get; init; } = GenreNames ?? [];
    public IReadOnlyList<string> StudioNames { get; init; } = StudioNames ?? [];
}

public interface IAnimeRepository
{
    Task<Anime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Anime>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<AnimeListItem>> GetAllAsync(int page, int pageSize, AnimeFilter filter, CancellationToken cancellationToken = default);
    Task AddAsync(Anime anime, CancellationToken cancellationToken = default);
    Task UpdateAsync(Anime anime, CancellationToken cancellationToken = default);
    Task DeleteAsync(Anime anime, CancellationToken cancellationToken = default);
}
