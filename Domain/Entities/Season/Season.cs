using Domain.Exceptions;

namespace Domain.Entities;

public partial class Season : Entity
{
    private Season() : base(Guid.NewGuid()) { } // EF Core

    public Season(
        Guid animeId,
        int seasonNumber,
        string title,
        string description)
        : base(Guid.NewGuid())
    {
        AnimeId = animeId;
        SeasonNumber = seasonNumber;
        Title = title ?? throw new ValidationException("Season title cannot be empty");
        Description = description ?? throw new ValidationException("Season description cannot be empty");

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public Guid AnimeId { get; private init; }

    public int SeasonNumber { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public int EpisodesCount { get; private set; } = 0;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Навігація
    public Anime Anime { get; private set; } = null!;
    public ICollection<Episode> Episodes { get; private set; } = new List<Episode>();
}
