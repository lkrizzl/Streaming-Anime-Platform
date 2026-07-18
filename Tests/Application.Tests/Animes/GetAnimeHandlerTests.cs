using System.Reflection;
using Application.Abstractions;
using Application.Animes;
using Domain;
using Domain.Associations;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class GetAnimeHandlerTests
{
    private readonly IAnimeRepository _animeRepository;
    private readonly GetAnimeHandler _handler;

    public GetAnimeHandlerTests()
    {
        _animeRepository = Substitute.For<IAnimeRepository>();
        _handler = new GetAnimeHandler(_animeRepository);
    }

    [Fact]
    public async Task Handle_WhenAnimeExists_ReturnsAnimeResponse()
    {
        var animeId = Guid.NewGuid();
        var anime = CreateAnime(animeId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);

        var result = await _handler.Handle(new GetAnimeQuery(animeId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(animeId, result.Id);
        Assert.Equal(anime.Title, result.Title);
        Assert.Equal(anime.Description, result.Description);
    }

    [Fact]
    public async Task Handle_WhenAnimeNotFound_ThrowsNotFoundException()
    {
        var animeId = Guid.NewGuid();
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _handler.Handle(new GetAnimeQuery(animeId), CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.NotFoundException>(act);
    }

    [Fact]
    public async Task Handle_ResponseHasCorrectGenres()
    {
        var animeId = Guid.NewGuid();
        var anime = CreateAnime(animeId);
        var genre = new Genre("Action");
        AddGenreToAnime(anime, genre);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);

        var result = await _handler.Handle(new GetAnimeQuery(animeId), CancellationToken.None);

        Assert.Contains("Action", result.Genres);
    }

    private static void AddGenreToAnime(Anime anime, Genre genre)
    {
        anime.AddGenre(genre);
        var animeGenre = anime.AnimeGenres.First(ag => ag.GenreId == genre.Id);
        typeof(AnimeGenre).GetProperty(nameof(AnimeGenre.Genre))!
            .SetValue(animeGenre, genre);
    }

    private static Anime CreateAnime(Guid? id = null)
    {
        var anime = new Anime("Test Anime", "Test Original", "Test Description", 2024, AnimeStatus.Airing);

        if (id.HasValue)
        {
            typeof(Entity).GetProperty("Id")!.SetValue(anime, id.Value);
        }

        return anime;
    }
}
