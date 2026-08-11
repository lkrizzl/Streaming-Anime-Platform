using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Genre : Entity
{
    private Genre() : base(Guid.NewGuid()) { }

    public Genre(GenreName name, string? description = null)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public GenreName Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;


    public ICollection<AnimeGenre> AnimeGenres { get; private set; } = new List<AnimeGenre>();

    public void UpdateName(GenreName newName)
    {
        Name = newName;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedOnUtc = UtcNow;
    }
}