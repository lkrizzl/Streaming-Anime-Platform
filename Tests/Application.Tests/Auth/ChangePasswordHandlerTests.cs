using Application.Abstractions;
using Application.Auth;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using NSubstitute;

namespace Application.Tests.Auth;

public class ChangePasswordHandlerTests
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserIdentityService _userIdentityService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ChangePasswordHandler _handler;

    public ChangePasswordHandlerTests()
    {
        _currentUser = Substitute.For<ICurrentUser>();
        _userIdentityService = Substitute.For<IUserIdentityService>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new ChangePasswordHandler(_currentUser, _userIdentityService, _passwordHasher, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ChangesPassword()
    {
        var userId = Guid.NewGuid();
        var _ = new User(userId, Username.Create("testuser"), Email.Create("test@example.com"));

        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");
        var userIdentity = new UserIdentity(
            userId,
            userId,
            Username.Create("testuser"),
            Email.Create("test@example.com"),
            Password.Create("OldPass1"),
            _passwordHasher
        );

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _userIdentityService.FindByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(userIdentity);
        _passwordHasher.VerifyPassword("OldPass1", userIdentity.PasswordHash).Returns(true);
        _passwordHasher.HashPassword("NewPass1").Returns("newHashedPassword");

        await _handler.Handle(
            new ChangePasswordCommand("OldPass1", "NewPass1"),
            CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsForbiddenException()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var act = async () => await _handler.Handle(
            new ChangePasswordCommand("OldPass1", "NewPass1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task Handle_WithWrongOldPassword_ThrowsBadRequestException()
    {
        var userId = Guid.NewGuid();
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");
        var userIdentity = new UserIdentity(
            userId,
            userId,
            Username.Create("testuser"),
            Email.Create("test@example.com"),
            Password.Create("OldPass1"),
            _passwordHasher
        );

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _userIdentityService.FindByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(userIdentity);
        _passwordHasher.VerifyPassword("WrongOldPass", userIdentity.PasswordHash).Returns(false);

        var act = async () => await _handler.Handle(
            new ChangePasswordCommand("WrongOldPass", "NewPass1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(act);
    }
}
