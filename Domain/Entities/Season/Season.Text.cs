using Domain.Exceptions;

namespace Domain.Entities;

// ====================== Текстові поля ======================
public partial class Season
{
    public void UpdateTitle(string newTitle)
    {
        Title = newTitle ?? throw new ValidationException("Season title cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? throw new ValidationException("Season description cannot be empty");
        UpdatedOnUtc = UtcNow;
    }
}
