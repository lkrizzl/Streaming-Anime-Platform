using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Anime
{
    public void UpdateTitle(Description title)
    {
        Title = title;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateOriginalTitle(Description originalTitle)
    {
        OriginalTitle = originalTitle;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateEnglishTitle(Description? englishTitle)
    {
        EnglishTitle = englishTitle;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedOnUtc = UtcNow;
    }
}
