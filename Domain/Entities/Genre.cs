using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Genre : Entity
{
    private Genre() : base(Guid.NewGuid()) { }

    public Genre(GenreName name, Synopsis? description = null)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public GenreName Name { get; private set; } = null!;
    public Synopsis? Description { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;


    private readonly List<AnimeGenre> _animeGenres = new();
    public IReadOnlyCollection<AnimeGenre> AnimeGenres => _animeGenres.AsReadOnly();

    public void UpdateName(GenreName newName)
    {
        Name = newName;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = Synopsis.CreateOptional(description);
        UpdatedOnUtc = UtcNow;
    }
}