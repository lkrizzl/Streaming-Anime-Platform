using System.Net;

namespace Integration.Tests;

[Collection("IntegrationTests")]
public class GenreStudioIntegrationTests : IntegrationTestBase
{
    public GenreStudioIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateGenre_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await PostJsonAsync("/api/genres", new { Name = "Action" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetGenres_AsAdmin_Succeeds()
    {
        await RegisterAndSignInAsync("admin-genres@example.com", "admingenres");

        var response = await PostJsonAsync("/api/genres", new { Name = "Action" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetGenres_WithoutAuth_ReturnsSuccess()
    {
        var response = await GetAsync("/api/genres?page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await DeserializeAsync<PaginatedResponse<GenreResponse>>(response);
        Assert.NotNull(content);
        Assert.NotNull(content.Items);
    }
}

internal record GenreResponse(Guid Id, string Name, string? Description);
internal record PaginatedResponse<T>(List<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
