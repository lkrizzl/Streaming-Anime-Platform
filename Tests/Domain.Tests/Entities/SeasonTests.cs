using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

public class SeasonTests
{
    private static readonly Guid AnimeId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var season = new Season(AnimeId, 1, "Season 1", "First season");

        Assert.NotEqual(Guid.Empty, season.Id);
        Assert.Equal(AnimeId, season.AnimeId);
        Assert.Equal(1, season.SeasonNumber);
        Assert.Equal("Season 1", season.Title);
        Assert.Equal("First season", season.Description);
        Assert.Equal(0, season.EpisodesCount);
        Assert.True(season.IsActive);
        Assert.Null(season.StartDate);
        Assert.Null(season.EndDate);
    }

    [Fact]
    public void Constructor_WithNullTitle_ThrowsValidationException()
    {
        var act = () => new Season(AnimeId, 1, null!, "Description");

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Constructor_WithNullDescription_ThrowsValidationException()
    {
        var act = () => new Season(AnimeId, 1, "Title", null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void AddEpisode_WithValidData_AddsEpisode()
    {
        var season = CreateDefaultSeason();

        var episode = season.AddEpisode(1, "Episode 1", TimeSpan.FromMinutes(24));

        Assert.NotNull(episode);
        Assert.Equal(1, episode.EpisodeNumber);
        Assert.Equal("Episode 1", episode.Title);
        Assert.Equal(season.Id, episode.SeasonId);
        Assert.Single(season.Episodes);
        Assert.Equal(1, season.EpisodesCount);
    }

    [Fact]
    public void AddEpisode_WithDuplicateNumber_ThrowsValidationException()
    {
        var season = CreateDefaultSeason();
        season.AddEpisode(1, "Episode 1", TimeSpan.FromMinutes(24));

        var act = () => season.AddEpisode(1, "Episode 1 Again", TimeSpan.FromMinutes(24));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void AddEpisode_MultipleEpisodes_IncrementsCount()
    {
        var season = CreateDefaultSeason();

        season.AddEpisode(1, "Episode 1", TimeSpan.FromMinutes(24));
        season.AddEpisode(2, "Episode 2", TimeSpan.FromMinutes(24));
        season.AddEpisode(3, "Episode 3", TimeSpan.FromMinutes(24));

        Assert.Equal(3, season.EpisodesCount);
        Assert.Equal(3, season.Episodes.Count);
    }

    [Fact]
    public void UpdateDates_WithValidDates_SetsDates()
    {
        var season = CreateDefaultSeason();
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 6, 1);

        season.UpdateDates(start, end);

        Assert.Equal(start, season.StartDate);
        Assert.Equal(end, season.EndDate);
    }

    [Fact]
    public void UpdateDates_WithNullDates_ClearsDates()
    {
        var season = CreateDefaultSeason();
        season.UpdateDates(new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1));

        season.UpdateDates(null, null);

        Assert.Null(season.StartDate);
        Assert.Null(season.EndDate);
    }

    [Fact]
    public void UpdateDates_WithEndBeforeStart_ThrowsValidationException()
    {
        var season = CreateDefaultSeason();

        var act = () => season.UpdateDates(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 1, 1));

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void IncrementEpisodeCount_IncreasesCount()
    {
        var season = CreateDefaultSeason();

        season.IncrementEpisodeCount();

        Assert.Equal(1, season.EpisodesCount);
    }

    private static Season CreateDefaultSeason()
        => new(AnimeId, 1, "Season 1", "First season");
}
