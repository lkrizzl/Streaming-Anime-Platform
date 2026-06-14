using Domain.Errors;
using Domain.Exceptions;

namespace Domain.Entities;

public class Genre : Entity
{
    private Genre() : base(Guid.NewGuid()) { } // EF Core

    public Genre(string name, string? description = null)
        : base(Guid.NewGuid())
    {
        Name = name ?? throw new ValidationException("Genre name cannot be empty");
        Description = description;

        CreatedOnUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public string Name { get; private set; }           // "Екшн", "Романтика", "Фентезі" тощо
    public string? Description { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    // ====================== Бізнес методи ======================

    public void UpdateName(string newName)
    {
        Name = newName ?? throw new ValidationException("Genre name cannot be empty");
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedOnUtc = DateTime.UtcNow;
    }
}