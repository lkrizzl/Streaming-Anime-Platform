using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class UserTests
{
    private static readonly Guid IdentityId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var username = Username.Create("john_doe");
        var email = Email.Create("john@example.com");
        var user = new User(IdentityId, username, email);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(IdentityId, user.IdentityId);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(UserRoles.User, user.Role);
        Assert.True(user.IsActive);
        Assert.False(user.IsBanned);
        Assert.Null(user.AvatarUrl);
        Assert.Null(user.Bio);
    }

    [Fact]
    public void Constructor_WithAdminRole_SetsAdminRole()
    {
        var user = new User(IdentityId, Username.Create("admin"), Email.Create("admin@example.com"));

        Assert.Equal(UserRoles.User, user.Role);
    }

    [Fact]
    public void UpdateUsername_WithValidValue_UpdatesProperty()
    {
        var user = CreateDefaultUser();
        var newUsername = Username.Create("new_john");

        user.UpdateUsername(newUsername);

        Assert.Equal(newUsername, user.Username);
    }

    [Fact]
    public void UpdateEmail_WithValidValue_UpdatesProperty()
    {
        var user = CreateDefaultUser();
        var newEmail = Email.Create("new@example.com");

        user.UpdateEmail(newEmail);

        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void UpdateAvatar_WithValidUrl_SetsAvatar()
    {
        var user = CreateDefaultUser();

        user.UpdateAvatar("https://example.com/avatar.jpg");

        Assert.Equal("https://example.com/avatar.jpg", user.AvatarUrl);
    }

    [Fact]
    public void UpdateAvatar_WithNull_ClearsAvatar()
    {
        var user = CreateDefaultUser();
        user.UpdateAvatar("https://example.com/avatar.jpg");

        user.UpdateAvatar(null);

        Assert.Null(user.AvatarUrl);
    }

    [Fact]
    public void UpdateBio_WithValidText_SetsBio()
    {
        var user = CreateDefaultUser();

        user.UpdateBio("Hello, I'm John!");

        Assert.Equal("Hello, I'm John!", user.Bio);
    }

    [Fact]
    public void UpdateBio_WithNull_ClearsBio()
    {
        var user = CreateDefaultUser();
        user.UpdateBio("Hello!");

        user.UpdateBio(null);

        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithWhitespace_ClearsBio()
    {
        var user = CreateDefaultUser();
        user.UpdateBio("Hello!");

        user.UpdateBio("   ");

        Assert.Null(user.Bio);
    }

    [Fact]
    public void RecordLogin_SetsLastLoginDate()
    {
        var user = CreateDefaultUser();

        user.RecordLogin();

        Assert.NotNull(user.LastLoginOnUtc);
        Assert.NotNull(user.UpdatedOnUtc);
    }

    private static User CreateDefaultUser()
        => new(IdentityId, Username.Create("john_doe"), Email.Create("john@example.com"));
}
