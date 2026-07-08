using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

public class AnimeTests
{
    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var anime = new Anime("Title", "Original", "Description", 2024, AnimeStatus.Airing);

        Assert.NotEqual(Guid.Empty, anime.Id);
        Assert.Equal("Title", anime.Title);
        Assert.Equal("Original", anime.OriginalTitle);
        Assert.Equal("Description", anime.Description);
        Assert.Equal(2024, anime.ReleaseYear);
        Assert.Equal(AnimeStatus.Airing, anime.Status);
        Assert.True(anime.IsActive);
        Assert.Equal(0, anime.AverageRating);
        Assert.Equal(0, anime.RatingCount);
    }

    [Fact]
    public void Constructor_WithNullTitle_ThrowsValidationException()
    {
        var act = () => new Anime(null!, "Original", "Description", 2024, AnimeStatus.Announced);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Constructor_WithNullOriginalTitle_ThrowsValidationException()
    {
        var act = () => new Anime("Title", null!, "Description", 2024, AnimeStatus.Announced);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void Constructor_WithNullDescription_ThrowsValidationException()
    {
        var act = () => new Anime("Title", "Original", null!, 2024, AnimeStatus.Announced);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateTitle_WithValidValue_UpdatesProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateTitle("New Title");

        Assert.Equal("New Title", anime.Title);
    }

    [Fact]
    public void UpdateTitle_WithNull_ThrowsValidationException()
    {
        var anime = CreateDefaultAnime();

        var act = () => anime.UpdateTitle(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateOriginalTitle_WithValidValue_UpdatesProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateOriginalTitle("New Original");

        Assert.Equal("New Original", anime.OriginalTitle);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_UpdatesProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateDescription("New Description");

        Assert.Equal("New Description", anime.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ThrowsValidationException()
    {
        var anime = CreateDefaultAnime();

        var act = () => anime.UpdateDescription(null!);

        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void UpdateEnglishTitle_WithValidValue_SetsProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateEnglishTitle("English Title");

        Assert.Equal("English Title", anime.EnglishTitle);
    }

    [Fact]
    public void UpdateEnglishTitle_WithNull_ClearsProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateEnglishTitle(null);

        Assert.Null(anime.EnglishTitle);
    }

    [Fact]
    public void UpdateEnglishTitle_WithWhitespace_ClearsProperty()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateEnglishTitle("   ");

        Assert.Null(anime.EnglishTitle);
    }

    [Fact]
    public void ChangeStatus_UpdatesStatus()
    {
        var anime = CreateDefaultAnime();

        anime.ChangeStatus(AnimeStatus.Completed);

        Assert.Equal(AnimeStatus.Completed, anime.Status);
    }

    [Fact]
    public void UpdateRating_SetsRatingAndCount()
    {
        var anime = CreateDefaultAnime();

        anime.UpdateRating(7.5, 100);

        Assert.Equal(7.5, anime.AverageRating);
        Assert.Equal(100, anime.RatingCount);
    }

    [Fact]
    public void SetCoverImage_WithValidUrl_SetsImage()
    {
        var anime = CreateDefaultAnime();

        anime.SetCoverImage("https://example.com/cover.jpg");

        Assert.Equal("https://example.com/cover.jpg", anime.CoverImageUrl);
    }

    [Fact]
    public void SetCoverImage_WithNull_ClearsImage()
    {
        var anime = CreateDefaultAnime();
        anime.SetCoverImage("https://example.com/cover.jpg");

        anime.SetCoverImage(null);

        Assert.Null(anime.CoverImageUrl);
    }

    [Fact]
    public void SetBannerImage_WithValidUrl_SetsImage()
    {
        var anime = CreateDefaultAnime();

        anime.SetBannerImage("https://example.com/banner.jpg");

        Assert.Equal("https://example.com/banner.jpg", anime.BannerImageUrl);
    }

    [Fact]
    public void SetAgeRating_WithValidValue_SetsRating()
    {
        var anime = CreateDefaultAnime();

        anime.SetAgeRating("18+");

        Assert.Equal("18+", anime.AgeRating);
    }

    [Fact]
    public void SetTrailerUrl_WithValidUrl_SetsTrailer()
    {
        var anime = CreateDefaultAnime();

        anime.SetTrailerUrl("https://example.com/trailer.mp4");

        Assert.Equal("https://example.com/trailer.mp4", anime.TrailerUrl);
    }

    private static Anime CreateDefaultAnime()
        => new("Title", "Original", "Description", 2024, AnimeStatus.Announced);
}
