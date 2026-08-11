using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class UserAnimeTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid AnimeId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var userAnime = new UserAnime(UserId, AnimeId, WatchStatus.Planned);

        Assert.NotEqual(Guid.Empty, userAnime.Id);
        Assert.Equal(UserId, userAnime.UserId);
        Assert.Equal(AnimeId, userAnime.AnimeId);
        Assert.Equal(WatchStatus.Planned, userAnime.Status);
        Assert.False(userAnime.IsFavorite);
        Assert.Null(userAnime.UserRating);
        Assert.Null(userAnime.LastWatchedEpisodeNumber);
        Assert.Null(userAnime.Notes);
    }

    [Fact]
    public void UpdateStatus_UpdatesStatus()
    {
        var userAnime = CreateDefault();

        userAnime.UpdateStatus(WatchStatus.Watching);

        Assert.Equal(WatchStatus.Watching, userAnime.Status);
    }

    [Fact]
    public void UpdateProgress_WithValidData_SetsProgress()
    {
        var userAnime = CreateDefault();

        userAnime.UpdateProgress(EpisodeNumber.Create(5), ProgressPercent.Create(50.0));

        Assert.Equal(5, userAnime.LastWatchedEpisodeNumber?.Value);
        Assert.Equal(50.0, userAnime.ProgressPercentage?.Value);
    }

    [Fact]
    public void UpdateProgress_WithEpisodeZero_ThrowsValidationException()
    {
        var userAnime = CreateDefault();

        var act = () => userAnime.UpdateProgress(EpisodeNumber.Create(0), ProgressPercent.Create(50.0));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateProgress_WithAboveMaxPercentage_ThrowsValidationException()
    {
        var userAnime = CreateDefault();

        var act = () => userAnime.UpdateProgress(EpisodeNumber.Create(10), ProgressPercent.Create(150.0));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateProgress_WithBelowMinPercentage_ThrowsValidationException()
    {
        var userAnime = CreateDefault();

        var act = () => userAnime.UpdateProgress(EpisodeNumber.Create(1), ProgressPercent.Create(-10.0));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Rate_WithValidRating_SetsRating()
    {
        var userAnime = CreateDefault();

        userAnime.Rate(Rating.Create(8.5));

        Assert.Equal(8.5, userAnime.UserRating?.Value);
    }

    [Fact]
    public void Rate_WithRatingBelowMin_ThrowsValidationException()
    {
        var userAnime = CreateDefault();

        var act = () => userAnime.Rate(Rating.Create(-0.5));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Rate_WithRatingAboveMax_ThrowsValidationException()
    {
        var userAnime = CreateDefault();

        var act = () => userAnime.Rate(Rating.Create(10.5));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void ToggleFavorite_FromFalse_BecomesTrue()
    {
        var userAnime = CreateDefault();

        userAnime.ToggleFavorite();

        Assert.True(userAnime.IsFavorite);
    }

    [Fact]
    public void ToggleFavorite_FromTrue_BecomesFalse()
    {
        var userAnime = CreateDefault();
        userAnime.ToggleFavorite();

        userAnime.ToggleFavorite();

        Assert.False(userAnime.IsFavorite);
    }

    [Fact]
    public void AddNote_WithValidText_SetsNote()
    {
        var userAnime = CreateDefault();

        userAnime.AddNote("Great anime!");

        Assert.Equal("Great anime!", userAnime.Notes);
    }

    [Fact]
    public void AddNote_WithNull_ClearsNote()
    {
        var userAnime = CreateDefault();
        userAnime.AddNote("Great anime!");

        userAnime.AddNote(null);

        Assert.Null(userAnime.Notes);
    }

    [Fact]
    public void AddNote_WithWhitespace_ClearsNote()
    {
        var userAnime = CreateDefault();
        userAnime.AddNote("Great anime!");

        userAnime.AddNote("   ");

        Assert.Null(userAnime.Notes);
    }

    private static UserAnime CreateDefault()
        => new(UserId, AnimeId, WatchStatus.Planned);
}
