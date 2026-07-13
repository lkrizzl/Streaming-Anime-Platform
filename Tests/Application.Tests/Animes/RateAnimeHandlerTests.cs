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

    [Fact]
    public async Task Handle_WithMixedRatedAndUnrated_CountsOnlyRated()
    {
        var userId = Guid.NewGuid();
        var animeId = Guid.NewGuid();
        var anime = new Anime("Test", "Original", "Description", 2024, AnimeStatus.Airing);

        // Створюємо UserAnime без оцінки (статус "Заплановане")
        var unratedUserAnime = new Domain.Associations.UserAnime(userId, animeId, WatchStatus.Planned);

        // Створюємо UserAnime з оцінкою
        var anotherUserId = Guid.NewGuid();
        var ratedUserAnime = new Domain.Associations.UserAnime(anotherUserId, animeId, WatchStatus.Completed);
        ratedUserAnime.Rate(7.0);

        var yetAnotherUserId = Guid.NewGuid();
        var ratedUserAnime2 = new Domain.Associations.UserAnime(yetAnotherUserId, animeId, WatchStatus.Watching);
        ratedUserAnime2.Rate(9.0);

        // Створюємо поточного користувача, який теж оцінює
        var currentUserAnime = new Domain.Associations.UserAnime(userId, animeId, WatchStatus.Planned);

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByUserAndAnimeAsync(userId, animeId, Arg.Any<CancellationToken>())
            .Returns(currentUserAnime);
        // currentUserAnime теж має бути в списку всіх записів для цього аніме
        _userAnimeRepository.GetByAnimeIdAsync(animeId, Arg.Any<CancellationToken>())
            .Returns(new List<Domain.Associations.UserAnime>
            {
                currentUserAnime,     // UserRating = 8.0 (після Rate(8.0))
                unratedUserAnime,     // UserRating = null — не має враховуватись
                ratedUserAnime,       // UserRating = 7.0
                ratedUserAnime2,      // UserRating = 9.0
            });

        await _handler.Handle(new RateAnimeCommand(animeId, 8.0), CancellationToken.None);

        // RatingCount має врахувати тільки ті, що мають UserRating.HasValue
        // (currentUserAnime (8.0) + ratedUserAnime (7.0) + ratedUserAnime2 (9.0) = 3)
        Assert.Equal(3, anime.RatingCount);
        // AverageRating: (8.0 + 7.0 + 9.0) / 3 = 8.0
        Assert.Equal(8.0, anime.AverageRating);
    }
}
