using Domain.Exceptions;
using Domain.Entities;

namespace Domain.Associations;

public enum WatchStatus
{
    Planned,
    Watching,
    Completed,
    OnHold,
    Dropped
}

public class UserAnime : Entity
{
    private UserAnime() : base(Guid.NewGuid()) { }

    public UserAnime(Guid userId, Guid animeId, WatchStatus status)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        AnimeId = animeId;
        Status = status;

        CreatedOnUtc = UtcNow;
        LastUpdatedOnUtc = UtcNow;
    }

    public Guid UserId { get; private init; }
    public Guid AnimeId { get; private init; }

    public WatchStatus Status { get; private set; }

    public int? LastWatchedEpisodeNumber { get; private set; }
    public double? ProgressPercentage { get; private set; }

    public double? UserRating { get; private set; }
    public string? Notes { get; private set; }

    public bool IsFavorite { get; private set; } = false;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime LastUpdatedOnUtc { get; private set; }

    // Навігація
    public User User { get; private set; } = null!;
    public Anime Anime { get; private set; } = null!;

    // ====================== Бізнес-методи ======================

    public void UpdateStatus(WatchStatus newStatus)
    {
        Status = newStatus;
        LastUpdatedOnUtc = UtcNow;
    }

    public void UpdateProgress(int episodeNumber, double progressPercentage)
    {
        if (episodeNumber < 1)
            throw new ValidationException("Episode number must be greater than 0.");

        LastWatchedEpisodeNumber = episodeNumber;
        ProgressPercentage = Math.Clamp(progressPercentage, 0, 100);
        LastUpdatedOnUtc = UtcNow;
    }

    public void Rate(double rating)
    {
        if (rating < 1 || rating > 10)
            throw new ValidationException("Rating must be between 1.0 and 10.0.");

        UserRating = rating;
        LastUpdatedOnUtc = UtcNow;
    }

    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        LastUpdatedOnUtc = UtcNow;
    }

    public void AddNote(string? note)
    {
        Notes = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        LastUpdatedOnUtc = UtcNow;
    }
}