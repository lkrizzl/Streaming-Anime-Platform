using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using Domain.ValueObjects.AnimeObjects;

namespace Domain.Entities;

public class Episode : Entity
{
    private Episode() : base(Guid.NewGuid()) { }

    public Episode(
        Guid seasonId,
        EpisodeNumber episodeNumber,
        Description title,
        TimeSpan duration)
        : base(Guid.NewGuid())
    {
        SeasonId = seasonId;
        EpisodeNumber = episodeNumber;
        Title = title;
        Duration = duration;

        CreatedOnUtc = UtcNow;
        IsActive = true;
        IsPublished = false;
    }

    public Guid SeasonId { get; private init; }

    public EpisodeNumber EpisodeNumber { get; private set; } = null!;
    public Description Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public TimeSpan Duration { get; private set; }
    public VideoUrl? VideoUrl { get; private set; }
    public ImageUrl? ThumbnailUrl { get; private set; }

    public DateTime? ReleaseDateUtc { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsPublished { get; private set; } = false;

    public Season Season { get; private set; } = null!;

    public void UpdateTitle(Description title)
    {
        Title = title;
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

    public void UpdateVideoUrl(Uri? videoUrl)
    {
        VideoUrl = VideoUrl.Create(videoUrl?.AbsoluteUri);
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateThumbnail(string? thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl is not null ? ImageUrl.Create(thumbnailUrl) : null;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ValidationException(EpisodeErrors.DurationMustBePositive());

        Duration = duration;
        UpdatedOnUtc = UtcNow;
    }
}