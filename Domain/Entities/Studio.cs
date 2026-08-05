using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Studio : Entity
{
    private Studio() : base(Guid.NewGuid()) { }

    public Studio(StudioName name, string? description = null)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public StudioName Name { get; private set; }
    public string? Description { get; private set; }
    public ImageUrl? LogoUrl { get; private set; }
    public Uri? WebsiteUrl { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<AnimeStudio> AnimeStudios { get; private set; } = new List<AnimeStudio>();

    public void UpdateName(StudioName newName)
    {
        Name = newName;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl is not null ? ImageUrl.Create(logoUrl) : null;
        UpdatedOnUtc = UtcNow;
    }
}