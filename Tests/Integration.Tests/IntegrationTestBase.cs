using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Tests;

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

        var serverHandler = factory.Server.CreateHandler();
        var innerHandler = new CookieAwareHandler(serverHandler, _cookieContainer);

        Client = new HttpClient(innerHandler)
        {
            BaseAddress = factory.Server.BaseAddress
        };
    }

    public async Task InitializeAsync()
    {
        await _factory.ApplyMigrationsAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    protected void ClearCookies()
    {
        _cookieContainer.GetAllCookies().ToList().ForEach(c => c.Expired = true);
    }

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

    protected async Task SignInAsync(string emailOrUsername, string password = "StrongPass1")
    {
        await PostJsonAsync("/auth/sign-in", new
        {
            UsernameOrEmail = emailOrUsername,
            Password = password,
            RememberMe = false
        });
    }

    protected async Task<HttpResponseMessage> PostJsonAsync(string url, object body)
    {
        return await Client.PostAsJsonAsync(url, body, JsonOptions);
    }

    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await Client.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }

    protected async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }
}

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
}

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
        var cookieHeader = _cookieContainer.GetCookieHeader(request.RequestUri!);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

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

                }
            }
        }

        return response;
    }
}
