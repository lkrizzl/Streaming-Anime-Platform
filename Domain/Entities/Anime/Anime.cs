using Domain.Associations;
using Domain.Exceptions;
using System.Linq;

namespace Domain.Entities;

public enum AnimeStatus
{
    Announced,
    Airing,
    Completed,
    Hiatus,
    Upcoming
}

public partial class Anime : Entity
{
    private Anime() : base(Guid.NewGuid()) { }

    public Anime(
        string title,
        string originalTitle,
        string description,
        int releaseYear,
        AnimeStatus status)
        : base(Guid.NewGuid())
    {
        Title = title ?? throw new ValidationException("Title cannot be empty");
        OriginalTitle = originalTitle ?? throw new ValidationException("Original title cannot be empty");
        Description = description ?? throw new ValidationException("Description cannot be empty");

        ReleaseYear = releaseYear;
        Status = status;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public string Title { get; private set; }
    public string OriginalTitle { get; private set; }
    public string? EnglishTitle { get; private set; }

    public string Description { get; private set; }

    public int ReleaseYear { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public AnimeStatus Status { get; private set; }

    // ====================== Images ======================
    public string? CoverImageUrl { get; private set; }
    public string? BannerImageUrl { get; private set; }
    public string? TrailerUrl { get; private set; }

    // ====================== Rating ======================
    public double AverageRating { get; private set; } = 0.0;
    public int RatingCount { get; private set; } = 0;

    // ====================== Episode tracking ======================
    public int EpisodesCount { get; private set; }
    public int CurrentEpisode { get; private set; } = 0;

    public string? AgeRating { get; private set; } = "16+";

    public bool IsActive { get; private set; } = true;
    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    // ====================== Navigation collections ======================
    public ICollection<AnimeGenre> AnimeGenres { get; private set; } = new List<AnimeGenre>();
    public ICollection<AnimeStudio> AnimeStudios { get; private set; } = new List<AnimeStudio>();
    public ICollection<Season> Seasons { get; private set; } = new List<Season>();
    public ICollection<UserAnime> UserAnimes { get; private set; } = new List<UserAnime>();

    // Read-only accessors
    public IReadOnlyCollection<Genre> Genres
        => AnimeGenres.Select(ag => ag.Genre).ToList();

    public IReadOnlyCollection<Studio> Studios
        => AnimeStudios.Select(ast => ast.Studio).ToList();
}
