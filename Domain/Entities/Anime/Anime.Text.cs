using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Anime
{
    public void UpdateTitle(Title title)
    {
        Title = title;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateOriginalTitle(Title originalTitle)
    {
        OriginalTitle = originalTitle;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateEnglishTitle(Title? englishTitle)
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
