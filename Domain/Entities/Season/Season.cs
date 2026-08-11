using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season : Entity
{
    private Season() : base(Guid.NewGuid()) { }

    public Season(
        Guid animeId,
        SeasonNumber seasonNumber,
        Description title,
        string description)
        : base(Guid.NewGuid())
    {
        AnimeId = animeId;
        SeasonNumber = seasonNumber;
        Title = title;
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public Guid AnimeId { get; private init; }

    public SeasonNumber SeasonNumber { get; private set; }
    public Description Title { get; private set; }
    public string Description { get; private set; }

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public int EpisodesCount { get; private set; } = 0;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Anime Anime { get; private set; } = null!;
    public ICollection<Episode> Episodes { get; private set; } = new List<Episode>();
}
