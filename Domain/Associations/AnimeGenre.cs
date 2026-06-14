using Domain.Entities;

namespace Domain.Associations;

public class AnimeGenre : Entity
{
    private AnimeGenre() : base(Guid.NewGuid()) { }

    public AnimeGenre(Guid animeId, Guid genreId) : base(Guid.NewGuid())
    {
        AnimeId = animeId;
        GenreId = genreId;
    }

    public Guid AnimeId { get; private init; }
    public Guid GenreId { get; private init; }

    public DateTime CreatedOnUtc { get; private init; } = DateTime.UtcNow;

    // Навігація
    public Anime Anime { get; private set; } = null!;
    public Genre Genre { get; private set; } = null!;
}