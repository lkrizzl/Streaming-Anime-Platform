using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

public class EpisodeTests
{
    private static readonly Guid SeasonId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var episode = new Episode(SeasonId, 1, "Episode 1", TimeSpan.FromMinutes(24));

        Assert.NotEqual(Guid.Empty, episode.Id);
        Assert.Equal(SeasonId, episode.SeasonId);
        Assert.Equal(1, episode.EpisodeNumber);
        Assert.Equal("Episode 1", episode.Title);
        Assert.Equal(TimeSpan.FromMinutes(24), episode.Duration);
        Assert.False(episode.IsPublished);
        Assert.True(episode.IsActive);
        Assert.Null(episode.VideoUrl);
        Assert.Null(episode.ReleaseDateUtc);
    }

    [Fact]
    public void Constructor_WithNullTitle_ThrowsValidationException()
    {
        var act = () => new Episode(SeasonId, 1, null!, TimeSpan.FromMinutes(24));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateTitle_WithValidValue_UpdatesProperty()
    {
        var episode = CreateDefaultEpisode();

        episode.UpdateTitle("New Title");

        Assert.Equal("New Title", episode.Title);
    }

    [Fact]
    public void UpdateTitle_WithNull_ThrowsValidationException()
    {
        var episode = CreateDefaultEpisode();

        var act = () => episode.UpdateTitle(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_SetsDescription()
    {
        var episode = CreateDefaultEpisode();

        episode.UpdateDescription("Some description");

        Assert.Equal("Some description", episode.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ClearsDescription()
    {
        var episode = CreateDefaultEpisode();
        episode.UpdateDescription("Some description");

        episode.UpdateDescription(null);

        Assert.Null(episode.Description);
    }

    [Fact]
    public void UpdateDescription_WithWhitespace_ClearsDescription()
    {
        var episode = CreateDefaultEpisode();
        episode.UpdateDescription("Some description");

        episode.UpdateDescription("   ");

        Assert.Null(episode.Description);
    }

    [Fact]
    public void Publish_SetsIsPublishedAndReleaseDate()
    {
        var episode = CreateDefaultEpisode();

        episode.Publish();

        Assert.True(episode.IsPublished);
        Assert.NotNull(episode.ReleaseDateUtc);
    }

    [Fact]
    public void Publish_WithSpecificDate_SetsReleaseDate()
    {
        var episode = CreateDefaultEpisode();
        var releaseDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc);

        episode.Publish(releaseDate);

        Assert.Equal(releaseDate, episode.ReleaseDateUtc);
    }

    [Fact]
    public void UpdateVideoUrl_WithValidUrl_SetsVideoUrl()
    {
        var episode = CreateDefaultEpisode();

        episode.UpdateVideoUrl("https://example.com/video.mp4");

        Assert.Equal("https://example.com/video.mp4", episode.VideoUrl);
    }

    [Fact]
    public void UpdateVideoUrl_WithNull_ThrowsValidationException()
    {
        var episode = CreateDefaultEpisode();

        var act = () => episode.UpdateVideoUrl(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateDuration_WithValidDuration_UpdatesDuration()
    {
        var episode = CreateDefaultEpisode();

        episode.UpdateDuration(TimeSpan.FromMinutes(30));

        Assert.Equal(TimeSpan.FromMinutes(30), episode.Duration);
    }

    [Fact]
    public void UpdateDuration_WithZero_ThrowsValidationException()
    {
        var episode = CreateDefaultEpisode();

        var act = () => episode.UpdateDuration(TimeSpan.Zero);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateDuration_WithNegative_ThrowsValidationException()
    {
        var episode = CreateDefaultEpisode();

        var act = () => episode.UpdateDuration(TimeSpan.FromMinutes(-1));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateThumbnail_SetsThumbnailUrl()
    {
        var episode = CreateDefaultEpisode();

        episode.UpdateThumbnail("https://example.com/thumb.jpg");

        Assert.Equal("https://example.com/thumb.jpg", episode.ThumbnailUrl);
    }

    private static Episode CreateDefaultEpisode()
        => new(SeasonId, 1, "Episode 1", TimeSpan.FromMinutes(24));
}
