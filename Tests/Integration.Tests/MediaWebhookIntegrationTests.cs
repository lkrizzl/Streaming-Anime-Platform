using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests;

[Collection("IntegrationTests")]
public class MediaWebhookIntegrationTests : IntegrationTestBase
{
    public MediaWebhookIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    private async Task<Guid> SeedEpisodeAsync()
    {
        Guid episodeId;
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var anime = new Anime(
                Description.Create("Webhook Test Anime", 200),
                Description.Create("Webhook Test Anime Original", 200),
                Synopsis.Create("For webhook testing"),
                ReleaseYear.Create(2024),
                AnimeStatus.Airing);

            context.Anime.Add(anime);
            await context.SaveChangesAsync();

            var season = new Season(
                anime.Id,
                SeasonNumber.Create(1),
                Description.Create("Season 1", 200),
                Synopsis.Create("Season 1 description"));

            context.Seasons.Add(season);
            await context.SaveChangesAsync();

            var episode = new Episode(
                season.Id,
                EpisodeNumber.Create(1),
                Description.Create("Webhook Episode", 200),
                TimeSpan.FromMinutes(24));

            context.Episodes.Add(episode);
            await context.SaveChangesAsync();

            episodeId = episode.Id;
        }

        return episodeId;
    }

    [Fact]
    public async Task MediaReady_WithAbsoluteUrl_ReturnsNoContent()
    {
        var episodeId = await SeedEpisodeAsync();
        var apiKey = "test-internal-api-key";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/internal/episodes/{episodeId}/media-ready")
        {
            Content = JsonContent.Create(new { StreamUrl = "http://localhost:5100/api/upload/episode/x/stream" })
        };
        request.Headers.Add("X-Internal-Api-Key", apiKey);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task MediaReady_WithRelativeUrl_ReturnsBadRequest()
    {
        var episodeId = await SeedEpisodeAsync();
        var apiKey = "test-internal-api-key";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/internal/episodes/{episodeId}/media-ready")
        {
            Content = JsonContent.Create(new { StreamUrl = "hls/episode/media/playlist.m3u8" })
        };
        request.Headers.Add("X-Internal-Api-Key", apiKey);

        var response = await Client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected BadRequest, got {response.StatusCode}. Body: {body}");
    }

    [Fact]
    public async Task MediaReady_WithWrongApiKey_ReturnsUnauthorized()
    {
        var episodeId = await SeedEpisodeAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/internal/episodes/{episodeId}/media-ready")
        {
            Content = JsonContent.Create(new { StreamUrl = "http://localhost:5100/api/upload/episode/x/stream" })
        };
        request.Headers.Add("X-Internal-Api-Key", "wrong-key");

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}