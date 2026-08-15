using Application.Abstractions;
using Application.Animes;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class RateAnimeHandlerTests
{
    private readonly ICurrentUser _currentUser;
    private readonly IAnimeRepository _animeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAnimeRepository _userAnimeRepository;
    private readonly IPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RateAnimeHandler _handler;

    public RateAnimeHandlerTests()
    {
        _currentUser = Substitute.For<ICurrentUser>();
        _animeRepository = Substitute.For<IAnimeRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _userAnimeRepository = Substitute.For<IUserAnimeRepository>();
        _publisher = Substitute.For<IPublisher>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RateAnimeHandler(_currentUser, _animeRepository, _userRepository, _userAnimeRepository, _publisher, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesWatchlistEntryAndRatesIt()
    {
        var userId = Guid.NewGuid();
        var animeId = Guid.NewGuid();
        var anime = new Anime(Description.Create("Test", 500), Description.Create("Original", 500), Synopsis.Create("Description"), ReleaseYear.Create(2024), AnimeStatus.Airing);
        var user = new User(Guid.NewGuid(), Username.Create("tester"), Email.Create("tester@test.com"));

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByUserAndAnimeAsync(userId, animeId, Arg.Any<CancellationToken>())
            .ReturnsNull();
        _userRepository.GetUserWithWatchlistAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        await _handler.Handle(new RateAnimeCommand(animeId, 8.0), CancellationToken.None);

        var createdUserAnime = user.UserAnimes.Single(ua => ua.AnimeId == animeId);
        Assert.Equal(8.0, createdUserAnime.UserRating?.Value);

        await _userRepository.Received(1).GetUserWithWatchlistAsync(userId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(
            Arg.Is<AnimeRatedNotification>(n => n.Event.AnimeId == animeId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsForbiddenException()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var act = async () => await _handler.Handle(new RateAnimeCommand(Guid.NewGuid(), 5.0), CancellationToken.None);

        await Assert.ThrowsAsync<Domain.Exceptions.ForbiddenException>(act);
        await _publisher.DidNotReceive().Publish(Arg.Any<AnimeRatedNotification>(), Arg.Any<CancellationToken>());
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
        await _publisher.DidNotReceive().Publish(Arg.Any<AnimeRatedNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UpdatesExistingUserAnimeRating()
    {
        var userId = Guid.NewGuid();
        var animeId = Guid.NewGuid();
        var anime = new Anime(Description.Create("Test", 500), Description.Create("Original", 500), Synopsis.Create("Description"), ReleaseYear.Create(2024), AnimeStatus.Airing);
        var existingUserAnime = new Domain.Entities.UserAnime(userId, animeId, WatchStatus.Watching);

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _animeRepository.GetByIdAsync(animeId, Arg.Any<CancellationToken>()).Returns(anime);
        _userAnimeRepository.GetByUserAndAnimeAsync(userId, animeId, Arg.Any<CancellationToken>())
            .Returns(existingUserAnime);

        await _handler.Handle(new RateAnimeCommand(animeId, 7.5), CancellationToken.None);

        Assert.Equal(7.5, existingUserAnime.UserRating?.Value);
        await _publisher.Received(1).Publish(
            Arg.Is<AnimeRatedNotification>(n => n.Event.AnimeId == animeId),
            Arg.Any<CancellationToken>());
    }
}