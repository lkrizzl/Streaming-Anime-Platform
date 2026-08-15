using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season : Entity
{
    private Season() : base(Guid.NewGuid()) { }

    internal Season(
        Guid animeId,
        SeasonNumber seasonNumber,
        Description title,
        Synopsis description)
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

    public SeasonNumber SeasonNumber { get; private set; } = null!;
    public Description Title { get; private set; } = null!;
    public Synopsis Description { get; private set; } = null!;

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public int EpisodesCount { get; private set; } = 0;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Anime Anime { get; private set; } = null!;

    private readonly List<Episode> _episodes = new();
    public IReadOnlyCollection<Episode> Episodes => _episodes.AsReadOnly();
}
