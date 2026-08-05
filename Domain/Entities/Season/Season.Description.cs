using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season
{
    public void UpdateTitle(Title newTitle)
    {
        Title = newTitle;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedOnUtc = UtcNow;
    }
}
