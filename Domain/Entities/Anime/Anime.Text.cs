using Domain.Exceptions;

namespace Domain.Entities;

// ====================== Текстові поля ======================
public partial class Anime
{
    public void UpdateTitle(string title)
    {
        Title = title ?? throw new ValidationException("Title cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateOriginalTitle(string originalTitle)
    {
        OriginalTitle = originalTitle ?? throw new ValidationException("Original title cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateEnglishTitle(string? englishTitle)
    {
        EnglishTitle = string.IsNullOrWhiteSpace(englishTitle) ? null : englishTitle.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? throw new ValidationException("Description cannot be empty");
        UpdatedOnUtc = UtcNow;
    }
}
