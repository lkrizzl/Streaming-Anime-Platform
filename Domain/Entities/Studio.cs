using Domain.Associations;
using Domain.Exceptions;

namespace Domain.Entities;

public class Studio : Entity
{
    private Studio() : base(Guid.NewGuid()) { } // EF Core

    public Studio(string name, string? description = null)
        : base(Guid.NewGuid())
    {
        Name = name ?? throw new ValidationException("Studio name cannot be empty");
        Description = description;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    public string Name { get; private set; }           // "MAPPA", "ufotable", "Kyoto Animation"
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Навігація
    public ICollection<AnimeStudio> AnimeStudios { get; private set; } = new List<AnimeStudio>();

    // ====================== Бізнес методи ======================

    public void UpdateName(string newName)
    {
        Name = newName ?? throw new ValidationException("Studio name cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;
        UpdatedOnUtc = UtcNow;
    }
}