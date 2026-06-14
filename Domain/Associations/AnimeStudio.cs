using Domain.Entities;

namespace Domain.Associations;

public class AnimeStudio : Entity
{
    private AnimeStudio() : base(Guid.NewGuid()) { }

    public AnimeStudio(Guid animeId, Guid studioId) : base(Guid.NewGuid())
    {
        AnimeId = animeId;
        StudioId = studioId;
    }

    public Guid AnimeId { get; private init; }
    public Guid StudioId { get; private init; }

    public DateTime CreatedOnUtc { get; private init; } = DateTime.UtcNow;

    // Навігація
    public Anime Anime { get; private set; } = null!;
    public Studio Studio { get; private set; } = null!;
}