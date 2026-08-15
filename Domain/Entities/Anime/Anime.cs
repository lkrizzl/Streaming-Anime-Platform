using Domain.ValueObjects;

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
        Description title,
        Description originalTitle,
        Synopsis description,
        ReleaseYear releaseYear,
        AnimeStatus status)
        : base(Guid.NewGuid())
    {
        Title = title;
        OriginalTitle = originalTitle;
        Description = description;
        ReleaseYear = releaseYear;
        Status = status;
        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public Description Title { get; private set; } = null!;
    public Description OriginalTitle { get; private set; } = null!;
    public Description? EnglishTitle { get; private set; }

    public Synopsis Description { get; private set; } = null!;

    public ReleaseYear ReleaseYear { get; private set; } = null!;
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public AnimeStatus Status { get; private set; }
    public ImageUrl? CoverImageUrl { get; private set; }
    public ImageUrl? BannerImageUrl { get; private set; }
    public ImageUrl? TrailerUrl { get; private set; }
    public Rating AverageRating { get; private set; } = Rating.Create(0.0);
    public int RatingCount { get; private set; } = 0;
    public int EpisodesCount { get; private set; }
    public int CurrentEpisode { get; private set; } = 0;

    public AgeRating AgeRating { get; private set; } = AgeRating.Default;

    public bool IsActive { get; private set; } = true;
    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public ICollection<AnimeGenre> AnimeGenres { get; private set; } = new List<AnimeGenre>();
    public ICollection<AnimeStudio> AnimeStudios { get; private set; } = new List<AnimeStudio>();
    public ICollection<Season> Seasons { get; private set; } = new List<Season>();
    public ICollection<UserAnime> UserAnimes { get; private set; } = new List<UserAnime>();

    public IReadOnlyCollection<Genre> Genres
        => AnimeGenres.Select(ag => ag.Genre).ToList();

    public IReadOnlyCollection<Studio> Studios
        => AnimeStudios.Select(ast => ast.Studio).ToList();
}
