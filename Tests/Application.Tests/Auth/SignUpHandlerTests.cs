using Application.Abstractions;
using Application.Auth;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Auth;

public class SignUpHandlerTests
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentityService _userIdentityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignUp _handler;

    public SignUpHandlerTests()
    {
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _userRepository = Substitute.For<IUserRepository>();
        _userIdentityService = Substitute.For<IUserIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new SignUp(_passwordHasher, _userRepository, _userIdentityService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesUser()
    {
        _userIdentityService.ExistsByEmailOrUsernameAsync(Arg.Any<Email>(), Arg.Any<Username>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _passwordHasher.HashPassword("StrongPass1").Returns("hashedPassword");
        _userIdentityService.AddAsync(Arg.Any<UserIdentity>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new SignUpCommand("test@example.com", "testuser", "StrongPass1"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(UserRoles.User, result.Role);
        Assert.NotEmpty(result.SecurityStamp);

        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _userIdentityService.Received(1).AddAsync(Arg.Any<UserIdentity>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsBadRequestException()
    {
        _userIdentityService.ExistsByEmailOrUsernameAsync(Arg.Any<Email>(), Arg.Any<Username>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var act = async () => await _handler.Handle(
            new SignUpCommand("existing@example.com", "testuser", "StrongPass1"),
            CancellationToken.None);

        var exception = await Assert.ThrowsAsync<BadRequestException>(act);
        Assert.Contains("already exists", exception.Message, StringComparison.OrdinalIgnoreCase);

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ThrowsEntityValidationException()
    {
        var act = async () => await _handler.Handle(
            new SignUpCommand("invalid-email", "testuser", "StrongPass1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<EntityValidationException>(act);
    }

    [Fact]
    public async Task Handle_WithShortPassword_ThrowsEntityValidationException()
    {
        var act = async () => await _handler.Handle(
            new SignUpCommand("test@example.com", "testuser", "Ab1"),
            CancellationToken.None);

        await Assert.ThrowsAsync<EntityValidationException>(act);
    }
}
