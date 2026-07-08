using System.Net;
using System.Net.Http.Json;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace Integration.Tests;

[Collection("IntegrationTests")]
public class AnimeIntegrationTests : IntegrationTestBase
{
    public AnimeIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAnime_WithoutAuth_ReturnsEmptyList()
    {
        var response = await GetAsync("/api/anime?page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await DeserializeAsync<PaginatedResponse<AnimeResponse>>(response);
        Assert.NotNull(content);
        Assert.Empty(content.Items);
    }

    [Fact]
    public async Task GetAnimeById_NotFound_Returns404()
    {
        var response = await GetAsync($"/api/anime/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAnime_WithoutAuth_ReturnsUnauthorized()
    {
        var command = new
        {
            Title = "Test Anime",
            OriginalTitle = "Test Original",
            EnglishTitle = (string?)null,
            Description = "Test Description",
            ReleaseYear = 2024,
            Status = "Announced",
            CoverImageUrl = (string?)null,
            BannerImageUrl = (string?)null,
            TrailerUrl = (string?)null,
            AgeRating = (string?)null,
            Genres = new List<string> { "Action" },
            Studios = new List<string> { "MAPPA" }
        };

        var response = await PostJsonAsync("/api/anime", command);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RateAnime_WithoutAuth_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/anime/{Guid.NewGuid()}/rate")
        {
            Content = JsonContent.Create(new { AnimeId = Guid.NewGuid(), Rating = 8.0 })
        };
        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task FullAnimeLifecycle_AsAdmin_Succeeds()
    {
        // Create admin user with genres and studios
        await CreateAdminUserAsync("admin-lifecycle@example.com", "adminlifecycle",
            genres: ["Action", "Comedy"],
            studios: ["MAPPA"]);

        // Create anime
        var createResponse = await PostJsonAsync("/api/anime", new
        {
            Title = "Lifecycle Anime",
            OriginalTitle = "Original Title",
            EnglishTitle = (string?)null,
            Description = "Anime for lifecycle testing",
            ReleaseYear = 2024,
            Status = "Announced",
            CoverImageUrl = (string?)null,
            BannerImageUrl = (string?)null,
            TrailerUrl = (string?)null,
            AgeRating = "16+",
            Genres = new List<string> { "Action", "Comedy" },
            Studios = new List<string> { "MAPPA" }
        });


        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await DeserializeAsync<AnimeResponse>(createResponse);
        Assert.NotNull(created);
        Assert.Equal("Lifecycle Anime", created.Title);

        // Get by ID
        var getResponse = await GetAsync($"/api/anime/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await DeserializeAsync<AnimeResponse>(getResponse);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);

        // Rate the anime (PUT, not POST)
        var rateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/anime/{created.Id}/rate")
        {
            Content = JsonContent.Create(new { AnimeId = created.Id, Rating = 8.5 }, options: JsonOptions)
        };
        var rateResponse = await Client.SendAsync(rateRequest);

        Assert.Equal(HttpStatusCode.NoContent, rateResponse.StatusCode);
    }

    /// <summary>
    /// Creates a user with Admin role directly in the database
    /// and optionally seeds genres and studios.
    /// </summary>
    private async Task CreateAdminUserAsync(string email, string username,
        string[]? genres = null, string[]? studios = null)
    {
        await RegisterAndSignInAsync(email, username);
        ClearCookies();

        // Seed genres and studios if provided
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (genres is not null)
            {
                foreach (var genreName in genres)
                {
                    if (!context.Genres.Any(g => g.Name == genreName))
                        context.Genres.Add(new Genre(genreName));
                }
            }

            if (studios is not null)
            {
                foreach (var studioName in studios)
                {
                    if (!context.Studios.Any(s => s.Name == studioName))
                        context.Studios.Add(new Studio(studioName));
                }
            }

            // Upgrade to Admin role in DB
            var user = context.Users.First(u => u.Email.Value == email);
            typeof(User).GetProperty("Role")!.SetValue(user, UserRoles.Admin);

            await context.SaveChangesAsync();
        }

        // Re-sign in to get admin cookie
        await SignInAsync(email);
    }

    [Fact]
    public async Task GetAllAnimes_WithPagination_ReturnsCorrectPage()
    {
        // Create admin and seed data
        await CreateAdminUserAsync("admin-pagination@example.com", "adminpagination",
            genres: ["Action"],
            studios: ["MAPPA"]);

        // Create 3 animes
        for (int i = 1; i <= 3; i++)
        {
            await PostJsonAsync("/api/anime", new
            {
                Title = $"Pagination Anime {i}",
                OriginalTitle = $"Original {i}",
                EnglishTitle = (string?)null,
                Description = $"Description {i}",
                ReleaseYear = 2024,
                Status = "Announced",
                Genres = new List<string> { "Action" },
                Studios = new List<string> { "MAPPA" }
            });
        }

        // Get page 1 with 2 items
        var response = await GetAsync("/api/anime?page=1&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page1 = await DeserializeAsync<PaginatedResponse<AnimeResponse>>(response);
        Assert.NotNull(page1);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(3, page1.TotalCount);
    }
}

internal record AnimeResponse(
    Guid Id,
    string Title,
    string OriginalTitle,
    string? EnglishTitle,
    string Description,
    int ReleaseYear,
    string Status,
    string? CoverImageUrl,
    string? BannerImageUrl,
    string? TrailerUrl,
    string? AgeRating,
    double AverageRating,
    int RatingCount,
    int EpisodesCount,
    bool IsActive,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc,
    List<string> Genres,
    List<string> Studios
);
