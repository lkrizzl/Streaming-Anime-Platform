using System.Net;

namespace Integration.Tests;

[Collection("IntegrationTests")]
public class AuthIntegrationTests : IntegrationTestBase
{
    public AuthIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task SignUp_WithValidData_ReturnsOk()
    {
        var response = await PostJsonAsync("/auth/sign-up", new
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "StrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_DuplicateEmail_ReturnsBadRequest()
    {
        await PostJsonAsync("/auth/sign-up", new
        {
            Email = "duplicate@example.com",
            Username = "firstuser",
            Password = "StrongPass1",
            RememberMe = false
        });

        var response = await PostJsonAsync("/auth/sign-up", new
        {
            Email = "duplicate@example.com",
            Username = "seconduser",
            Password = "StrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_WithWeakPassword_ReturnsBadRequest()
    {
        var response = await PostJsonAsync("/auth/sign-up", new
        {
            Email = "weak@example.com",
            Username = "weakuser",
            Password = "123",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ReturnsOk()
    {
        await RegisterAndSignInAsync("signin@example.com", "signinuser");
        ClearCookies();

        var response = await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = "signin@example.com",
            Password = "StrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WithWrongPassword_ReturnsBadRequest()
    {
        await PostJsonAsync("/auth/sign-up", new
        {
            Email = "wrongpass@example.com",
            Username = "wrongpassuser",
            Password = "StrongPass1",
            RememberMe = false
        });

        ClearCookies();
        var response = await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = "wrongpass@example.com",
            Password = "WrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WithNonexistentUser_ReturnsBadRequest()
    {
        var response = await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = "nonexistent@example.com",
            Password = "StrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WithUsername_ReturnsOk()
    {
        await PostJsonAsync("/auth/sign-up", new
        {
            Email = "byusername@example.com",
            Username = "loginbyuser",
            Password = "StrongPass1",
            RememberMe = false
        });
        ClearCookies();

        var response = await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = "loginbyuser",
            Password = "StrongPass1",
            RememberMe = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_AfterSignIn_ReturnsOk()
    {
        await RegisterAndSignInAsync("me@example.com", "meuser");

        var response = await GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await DeserializeAsync<MeResponse>(response);
        Assert.NotNull(content);
        Assert.Equal("meuser", content.Username);
        Assert.Equal("me@example.com", content.Email);
    }

    [Fact]
    public async Task SignOut_AfterSignIn_ClearsSession()
    {
        await RegisterAndSignInAsync("signout@example.com", "signoutuser");

        var response = await PostJsonAsync("/auth/sign-out", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ClearCookies();

        var meResponse = await GetAsync("/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }
}

internal record MeResponse(
    Guid Id,
    string Email,
    string Username,
    string AvatarUrl,
    string Bio,
    string Role
);
