using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season
{
    public void UpdateTitle(Description newTitle)
    {
        Title = newTitle;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = Synopsis.Create(description);
        UpdatedOnUtc = UtcNow;
    }
}
