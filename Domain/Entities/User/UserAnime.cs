using Domain.ValueObjects;

namespace Domain.Entities;

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

    internal UserAnime(Guid userId, Guid animeId, WatchStatus status)
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

    public EpisodeNumber? LastWatchedEpisodeNumber { get; private set; }
    public ProgressPercent? ProgressPercentage { get; private set; }

    public Rating? UserRating { get; private set; }
    public string? Notes { get; private set; }

    public bool IsFavorite { get; private set; } = false;

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime LastUpdatedOnUtc { get; private set; }
    public User User { get; private set; } = null!;
    public Anime Anime { get; private set; } = null!;

    public void UpdateStatus(WatchStatus newStatus)
    {
        Status = newStatus;
        LastUpdatedOnUtc = UtcNow;
    }

    public void UpdateProgress(EpisodeNumber? episodeNumber, ProgressPercent? progressPercentage)
    {
        LastWatchedEpisodeNumber = episodeNumber;
        ProgressPercentage = progressPercentage;
        LastUpdatedOnUtc = UtcNow;
    }

    public void Rate(Rating? rating)
    {
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