using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Studio : Entity
{
    private Studio() : base(Guid.NewGuid()) { }

    public Studio(StudioName name, Synopsis? description = null)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public StudioName Name { get; private set; } = null!;
    public Synopsis? Description { get; private set; }
    public ImageUrl? LogoUrl { get; private set; }
    public Uri? WebsiteUrl { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<AnimeStudio> _animeStudios = new();
    public IReadOnlyCollection<AnimeStudio> AnimeStudios => _animeStudios.AsReadOnly();

    public void UpdateName(StudioName newName)
    {
        Name = newName;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = Synopsis.CreateOptional(description);
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl is not null ? ImageUrl.Create(logoUrl) : null;
        UpdatedOnUtc = UtcNow;
    }
}