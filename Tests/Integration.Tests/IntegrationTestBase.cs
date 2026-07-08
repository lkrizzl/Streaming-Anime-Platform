using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Tests;

/// <summary>
/// Base class for integration tests. Initializes the TestContainer and WebApplicationFactory.
/// </summary>
[Collection("IntegrationTests")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly CookieContainer _cookieContainer = new();

    protected HttpClient Client { get; }
    protected IntegrationTestWebAppFactory Factory => _factory;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;

        // Create a handler chain: cookie handler → server handler
        var serverHandler = factory.Server.CreateHandler();
        var innerHandler = new CookieAwareHandler(serverHandler, _cookieContainer);

        Client = new HttpClient(innerHandler)
        {
            BaseAddress = factory.Server.BaseAddress
        };
    }

    public async Task InitializeAsync()
    {
        // Ensure migrations are applied before any test
        await _factory.ApplyMigrationsAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up database for the next test
        await _factory.ResetDatabaseAsync();
    }

    /// <summary>
    /// Clears all cookies, effectively logging out.
    /// </summary>
    protected void ClearCookies()
    {
        _cookieContainer.GetAllCookies().ToList().ForEach(c => c.Expired = true);
    }

    /// <summary>
    /// Registers and signs in a test user, returning the client with auth cookie.
    /// </summary>
    protected async Task RegisterAndSignInAsync(string email, string username, string password = "StrongPass1")
    {
        await PostJsonAsync("/auth/sign-up", new
        {
            Email = email,
            Username = username,
            Password = password,
            RememberMe = false
        });
    }

    /// <summary>
    /// Signs in an existing user.
    /// </summary>
    protected async Task SignInAsync(string emailOrUsername, string password = "StrongPass1")
    {
        await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = emailOrUsername,
            Password = password,
            RememberMe = false
        });
    }

    /// <summary>
    /// Sends a POST request with a JSON body and returns the response.
    /// </summary>
    protected async Task<HttpResponseMessage> PostJsonAsync(string url, object body)
    {
        return await Client.PostAsJsonAsync(url, body, JsonOptions);
    }

    /// <summary>
    /// Sends a GET request and returns the response.
    /// </summary>
    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await Client.GetAsync(url);
    }

    /// <summary>
    /// Sends a DELETE request and returns the response.
    /// </summary>
    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }

    /// <summary>
    /// Deserializes response content to the specified type.
    /// </summary>
    protected async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }
}

/// <summary>
/// Collection definition for integration tests.
/// Ensures the factory is shared across all test classes in the collection.
/// </summary>
[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
}

/// <summary>
/// A DelegatingHandler that manages cookies for integration tests.
/// Extracts Set-Cookie headers from responses and sends them back in requests.
/// </summary>
internal class CookieAwareHandler : DelegatingHandler
{
    private readonly CookieContainer _cookieContainer;

    public CookieAwareHandler(HttpMessageHandler innerHandler, CookieContainer cookieContainer)
        : base(innerHandler)
    {
        _cookieContainer = cookieContainer;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Attach stored cookies
        var cookieHeader = _cookieContainer.GetCookieHeader(request.RequestUri!);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Extract and store cookies from response
        if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
        {
            foreach (var setCookieValue in setCookieHeaders)
            {
                try
                {
                    _cookieContainer.SetCookies(response.RequestMessage!.RequestUri!, setCookieValue);
                }
                catch
                {
                    // Skip invalid cookies
                }
            }
        }

        return response;
    }
}
