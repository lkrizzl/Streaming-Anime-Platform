using Domain.Errors;
using Domain.Exceptions;

namespace Domain.Entities;

public class Season : Entity
{
    private Season() : base(Guid.NewGuid()) { } // EF Core

    public Season(
        Guid animeId,
        int seasonNumber,
        string title,
        string description)
        : base(Guid.NewGuid())
    {
        AnimeId = animeId;
        SeasonNumber = seasonNumber;
        Title = title ?? throw new ValidationException("Season title cannot be empty");
        Description = description ?? throw new ValidationException("Season description cannot be empty");

        CreatedOnUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid AnimeId { get; private init; }

    public int SeasonNumber { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public int EpisodesCount { get; private set; } = 0;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Навігація
    public Anime Anime { get; private set; } = null!;
    public ICollection<Episode> Episodes { get; private set; } = new List<Episode>();

    // ====================== Бізнес методи ======================

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle ?? throw new ValidationException("Season title cannot be empty");
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateDates(DateOnly? startDate, DateOnly? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void IncrementEpisodeCount()
    {
        EpisodesCount++;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}