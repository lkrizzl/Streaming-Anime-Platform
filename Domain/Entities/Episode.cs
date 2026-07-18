using Domain.Exceptions;

namespace Domain.Entities;

public class Episode : Entity
{
    private Episode() : base(Guid.NewGuid()) { }

    public Episode(
        Guid seasonId,
        int episodeNumber,
        string title,
        TimeSpan duration)
        : base(Guid.NewGuid())
    {
        SeasonId = seasonId;
        EpisodeNumber = episodeNumber;
        Title = title ?? throw new ValidationException("Episode title cannot be empty");
        Duration = duration;

        CreatedOnUtc = UtcNow;
        IsActive = true;
        IsPublished = false;
    }

    public Guid SeasonId { get; private init; }

    public int EpisodeNumber { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }

    public TimeSpan Duration { get; private set; }
    public Uri VideoUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }

    public DateTime? ReleaseDateUtc { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsPublished { get; private set; } = false;

    // Навігація
    public Season Season { get; private set; } = null!;

    public void UpdateTitle(string title)
    {
        Title = title ?? throw new ValidationException("Episode title cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void Publish(DateTime? releaseDate = null)
    {
        IsPublished = true;
        ReleaseDateUtc = releaseDate ?? UtcNow;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateVideoUrl(Uri videoUrl)
    {
        VideoUrl = videoUrl ?? throw new ValidationException("Video URL cannot be empty");
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateThumbnail(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ValidationException("Duration must be greater than zero.");
        Duration = duration;
        UpdatedOnUtc = UtcNow;
    }
}