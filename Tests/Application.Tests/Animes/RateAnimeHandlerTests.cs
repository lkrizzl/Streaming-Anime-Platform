using Application.Abstractions;
using Application.Animes;
using Domain.Abstractions;
using Domain.Associations;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class RateAnimeHandlerTests
{
    private readonly ICurrentUser _currentUser;
    private readonly IAnimeRepository _animeRepository;
    private readonly IUserAnimeRepository _userAnimeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RateAnimeHandler _handler;

    public RateAnimeHandlerTests()
    {
        _currentUser = Substitute.For<ICurrentUser>();
        _animeRepository = Substitute.For<IAnimeRepository>();
        _userAnimeRepository = Substitute.For<IUserAnimeRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RateAnimeHandler(_currentUser, _animeRepository, _userAnimeRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_RatesAnime()
    {
        var userId = Guid.NewGuid();
        var animeId = Guid.NewGuid();
        var anime = new Anime("Test", "Original", "Description", 2024, AnimeStatus.Airing);

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByUserAndAnimeAsync(userId, animeId, Arg.Any<CancellationToken>())
            .ReturnsNull();
        var ratedUserAnime = new Domain.Associations.UserAnime(userId, animeId, WatchStatus.Planned);
        ratedUserAnime.Rate(8.0);
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Associations.UserAnime>
            {
                ratedUserAnime
            });

        await _handler.Handle(new RateAnimeCommand(animeId, 8.0), CancellationToken.None);

        await _userAnimeRepository.Received(1).AddAsync(Arg.Any<Domain.Associations.UserAnime>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal(8.0, anime.AverageRating);
        Assert.Equal(1, anime.RatingCount);
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsForbiddenException()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var act = async () => await _handler.Handle(new RateAnimeCommand(Guid.NewGuid(), 5.0), CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.ForbiddenException>(act);
    }

    [Fact]
    public async Task Handle_WhenAnimeNotFound_ThrowsNotFoundException()
    {
        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _animeRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(new RateAnimeCommand(Guid.NewGuid(), 5.0), CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.NotFoundException>(act);
    }

    [Fact]
    public async Task Handle_UpdatesExistingUserAnimeRating()
    {
        var userId = Guid.NewGuid();
        var animeId = Guid.NewGuid();
        var anime = new Anime("Test", "Original", "Description", 2024, AnimeStatus.Airing);
        var existingUserAnime = new Domain.Associations.UserAnime(userId, animeId, WatchStatus.Watching);

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByUserAndAnimeAsync(userId, animeId, Arg.Any<CancellationToken>())
            .Returns(existingUserAnime);
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Associations.UserAnime>
            {
                existingUserAnime
            });

        await _handler.Handle(new RateAnimeCommand(animeId, 7.5), CancellationToken.None);

        Assert.Equal(7.5, existingUserAnime.UserRating);
        await _userAnimeRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Associations.UserAnime>(), Arg.Any<CancellationToken>());
    }
}
