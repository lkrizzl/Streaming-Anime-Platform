using Application.Abstractions;
using Application.Animes;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class RecalculateAnimeRatingHandlerTests
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IUserAnimeRepository _userAnimeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RecalculateAnimeRatingHandler _handler;

    public RecalculateAnimeRatingHandlerTests()
    {
        _animeRepository = Substitute.For<IAnimeRepository>();
        _userAnimeRepository = Substitute.For<IUserAnimeRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RecalculateAnimeRatingHandler(_animeRepository, _userAnimeRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithMixedRatedAndUnrated_CountsOnlyRated()
    {
        var animeId = Guid.NewGuid();
        var anime = new Anime(Description.Create("Test", 500), Description.Create("Original", 500), "Description", ReleaseYear.Create(2024), AnimeStatus.Airing);

        var unratedUserAnime = new Domain.Entities.UserAnime(Guid.NewGuid(), animeId, WatchStatus.Planned);

        var ratedUserAnime = new Domain.Entities.UserAnime(Guid.NewGuid(), animeId, WatchStatus.Completed);
        ratedUserAnime.Rate(Rating.Create(7.0));

        var ratedUserAnime2 = new Domain.Entities.UserAnime(Guid.NewGuid(), animeId, WatchStatus.Watching);
        ratedUserAnime2.Rate(Rating.Create(9.0));

        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Entities.UserAnime>
            {
                unratedUserAnime,
                ratedUserAnime,
                ratedUserAnime2,
            });

        await _handler.Handle(new AnimeRatedNotification(new AnimeRatedEvent(animeId)), CancellationToken.None);

        Assert.Equal(2, anime.RatingCount);
        Assert.Equal(8.0, anime.AverageRating);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithSingleRating_SetsAverageToThatRating()
    {
        var animeId = Guid.NewGuid();
        var anime = new Anime(Description.Create("Test", 500), Description.Create("Original", 500), "Description", ReleaseYear.Create(2024), AnimeStatus.Airing);

        var ratedUserAnime = new Domain.Entities.UserAnime(Guid.NewGuid(), animeId, WatchStatus.Planned);
        ratedUserAnime.Rate(Rating.Create(8.0));

        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Entities.UserAnime> { ratedUserAnime });

        await _handler.Handle(new AnimeRatedNotification(new AnimeRatedEvent(animeId)), CancellationToken.None);

        Assert.Equal(1, anime.RatingCount);
        Assert.Equal(8.0, anime.AverageRating);
    }

    [Fact]
    public async Task Handle_WithNoRatings_SetsZeroAverageAndZeroCount()
    {
        var animeId = Guid.NewGuid();
        var anime = new Anime(Description.Create("Test", 500), Description.Create("Original", 500), "Description", ReleaseYear.Create(2024), AnimeStatus.Airing);

        var unratedUserAnime = new Domain.Entities.UserAnime(Guid.NewGuid(), animeId, WatchStatus.Planned);

        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Entities.UserAnime> { unratedUserAnime });

        await _handler.Handle(new AnimeRatedNotification(new AnimeRatedEvent(animeId)), CancellationToken.None);

        Assert.Equal(0, anime.RatingCount);
        Assert.Equal(0.0, anime.AverageRating);
    }

    [Fact]
    public async Task Handle_WhenAnimeNotFound_DoesNothingAndDoesNotSave()
    {
        var animeId = Guid.NewGuid();

        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).ReturnsNull();

        await _handler.Handle(new AnimeRatedNotification(new AnimeRatedEvent(animeId)), CancellationToken.None);

        await _userAnimeRepository.DidNotReceive().GetByAnimeIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}