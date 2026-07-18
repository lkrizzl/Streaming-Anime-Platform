using System.Reflection;
using Application.Abstractions;
using Application.Animes;
using Domain.Associations;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class CreateAnimeHandlerTests
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IStudioRepository _studioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateAnimeHandler _handler;
    private readonly Dictionary<Guid, Genre> _knownGenres = new();
    private readonly Dictionary<Guid, Studio> _knownStudios = new();

    public CreateAnimeHandlerTests()
    {
        _animeRepository = Substitute.For<IAnimeRepository>();
        _genreRepository = Substitute.For<IGenreRepository>();
        _studioRepository = Substitute.For<IStudioRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _animeRepository
            .When(r => r.AddAsync(Arg.Any<Anime>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                var anime = callInfo.Arg<Anime>();
                FixUpNavigations(anime);
            });

        _handler = new CreateAnimeHandler(_animeRepository, _genreRepository, _studioRepository, _unitOfWork);
    }

    private void FixUpNavigations(Anime anime)
    {
        foreach (var ag in anime.AnimeGenres)
        {
            if (ag.Genre is null && _knownGenres.TryGetValue(ag.GenreId, out var genre))
            {
                typeof(AnimeGenre).GetProperty(nameof(AnimeGenre.Genre))!
                    .SetValue(ag, genre);
            }
        }

        foreach (var ast in anime.AnimeStudios)
        {
            if (ast.Studio is null && _knownStudios.TryGetValue(ast.StudioId, out var studio))
            {
                typeof(AnimeStudio).GetProperty(nameof(AnimeStudio.Studio))!
                    .SetValue(ast, studio);
            }
        }
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesAnime()
    {
        var actionGenre = new Genre("Action");
        var comedyGenre = new Genre("Comedy");
        var mappaStudio = new Studio("MAPPA");

        _knownGenres[actionGenre.Id] = actionGenre;
        _knownGenres[comedyGenre.Id] = comedyGenre;
        _knownStudios[mappaStudio.Id] = mappaStudio;

        _genreRepository.GetByNameAsync("Action", Arg.Any<CancellationToken>())
            .Returns(actionGenre);
        _genreRepository.GetByNameAsync("Comedy", Arg.Any<CancellationToken>())
            .Returns(comedyGenre);
        _studioRepository.GetByNameAsync("MAPPA", Arg.Any<CancellationToken>())
            .Returns(mappaStudio);

        var command = new CreateAnimeCommand(
            Title: "Test Anime",
            OriginalTitle: "Test Original",
            EnglishTitle: "English Title",
            Description: "Test Description",
            ReleaseYear: 2024,
            Status: AnimeStatus.Airing,
            CoverImageUrl: "https://example.com/cover.jpg",
            BannerImageUrl: null,
            TrailerUrl: null,
            AgeRating: "16+",
            Genres: new List<string> { "Action", "Comedy" },
            Studios: new List<string> { "MAPPA" }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Test Anime", result.Title);
        Assert.Equal("English Title", result.EnglishTitle);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(2024, result.ReleaseYear);
        Assert.Equal("https://example.com/cover.jpg", result.CoverImageUrl);
        Assert.Equal("16+", result.AgeRating);
        Assert.Contains("Action", result.Genres);
        Assert.Contains("Comedy", result.Genres);
        Assert.Contains("MAPPA", result.Studios);

        await _genreRepository.Received(1).GetByNameAsync("Action", Arg.Any<CancellationToken>());
        await _genreRepository.Received(1).GetByNameAsync("Comedy", Arg.Any<CancellationToken>());
        await _studioRepository.Received(1).GetByNameAsync("MAPPA", Arg.Any<CancellationToken>());
        await _animeRepository.Received(1).AddAsync(Arg.Any<Anime>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGenreNotFound_ThrowsNotFoundException()
    {
        _genreRepository.GetByNameAsync("NonExistent", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var command = new CreateAnimeCommand(
            Title: "Test",
            OriginalTitle: "Test",
            EnglishTitle: null,
            Description: "Test",
            ReleaseYear: 2024,
            Status: AnimeStatus.Announced,
            CoverImageUrl: null,
            BannerImageUrl: null,
            TrailerUrl: null,
            AgeRating: null,
            Genres: new List<string> { "NonExistent" },
            Studios: new List<string> { "MAPPA" }
        );

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.NotFoundException>(act);
    }

    [Fact]
    public async Task Handle_WhenStudioNotFound_ThrowsNotFoundException()
    {
        _genreRepository.GetByNameAsync("Action", Arg.Any<CancellationToken>())
            .Returns(new Genre("Action"));
        _studioRepository.GetByNameAsync("NonExistent", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var command = new CreateAnimeCommand(
            Title: "Test",
            OriginalTitle: "Test",
            EnglishTitle: null,
            Description: "Test",
            ReleaseYear: 2024,
            Status: AnimeStatus.Announced,
            CoverImageUrl: null,
            BannerImageUrl: null,
            TrailerUrl: null,
            AgeRating: null,
            Genres: new List<string> { "Action" },
            Studios: new List<string> { "NonExistent" }
        );

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.NotFoundException>(act);
    }
}
