using Application.Abstractions;
using Application.Auth;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Auth;

public class SignInHandlerTests
{
    private readonly IUserIdentityService _userIdentityService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SignIn _handler;

    public SignInHandlerTests()
    {
        _userIdentityService = Substitute.For<IUserIdentityService>();
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new SignIn(_userIdentityService, _userRepository, _passwordHasher);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSignInResponse()
    {
        var userId = Guid.NewGuid();
        var userIdentityUserId = Guid.NewGuid();
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");
        var userIdentity = new UserIdentity(
            userIdentityUserId,
            userId,
            Username.Create("testuser"),
            Email.Create("test@example.com"),
            Password.Create("StrongPass1"),
            _passwordHasher
        );
        var user = new User(userId, Username.Create("testuser"), Email.Create("test@example.com"));

        _userIdentityService.FindByEmailOrUsernameAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(userIdentity);
        _passwordHasher.VerifyPassword("StrongPass1", userIdentity.PasswordHash).Returns(true);
        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(
            new SignInCommand("test@example.com", "StrongPass1"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ThrowsBadRequestException()
    {
        _userIdentityService.FindByEmailOrUsernameAsync("unknown@example.com", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _handler.Handle(
            new SignInCommand("unknown@example.com", "StrongPass1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(act);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsBadRequestException()
    {
        _passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword");
        var userIdentity = new UserIdentity(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Username.Create("testuser"),
            Email.Create("test@example.com"),
            Password.Create("StrongPass1"),
            _passwordHasher
        );

        _userIdentityService.FindByEmailOrUsernameAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(userIdentity);
        _passwordHasher.VerifyPassword("WrongPass1", userIdentity.PasswordHash).Returns(false);

        var act = async () => await _handler.Handle(
            new SignInCommand("test@example.com", "WrongPass1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(act);
    }
}
