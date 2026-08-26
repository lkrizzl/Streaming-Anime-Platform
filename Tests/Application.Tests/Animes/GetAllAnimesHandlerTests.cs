using Application.Abstractions;
using Application.Animes;
using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Animes;

public class GetAllAnimesHandlerTests
{
    private readonly IAnimeRepository _animeRepository;
    private readonly GetAllAnimesHandler _handler;

    public GetAllAnimesHandlerTests()
    {
        _animeRepository = Substitute.For<IAnimeRepository>();
        _handler = new GetAllAnimesHandler(_animeRepository);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedList()
    {
        var anime = CreateAnimeListItem();
        var paginated = new PaginatedList<AnimeListItem>(new[] { anime }, 1, 20, 1);
        _animeRepository.GetAllAsync(1, 20, Arg.Any<AnimeFilter>(), Arg.Any<CancellationToken>())
            .Returns(paginated);

        var result = await _handler.Handle(new GetAllAnimesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(anime.Title, result.Items[0].Title);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var allAnimes = Enumerable.Range(1, 5)
            .Select(i => CreateAnimeListItem(title: $"Anime {i}"))
            .ToList();
        var paginated = new PaginatedList<AnimeListItem>(allAnimes, 1, 5, 5);
        _animeRepository.GetAllAsync(1, 5, Arg.Any<AnimeFilter>(), Arg.Any<CancellationToken>())
            .Returns(paginated);

        var result = await _handler.Handle(new GetAllAnimesQuery(Page: 1, PageSize: 5), CancellationToken.None);

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(5, result.TotalCount);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        var paginated = new PaginatedList<AnimeListItem>(new List<AnimeListItem>(), 1, 20, 0);
        _animeRepository.GetAllAsync(1, 20, Arg.Any<AnimeFilter>(), Arg.Any<CancellationToken>())
            .Returns(paginated);

        var result = await _handler.Handle(new GetAllAnimesQuery(), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_PassesFilterToRepository()
    {
        AnimeFilter? capturedFilter = null;
        var paginated = new PaginatedList<AnimeListItem>(new List<AnimeListItem>(), 1, 20, 0);
        _animeRepository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<AnimeFilter>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(paginated)
            .AndDoes(callInfo => capturedFilter = callInfo.Arg<AnimeFilter>());

        await _handler.Handle(new GetAllAnimesQuery(
            Search: "naruto",
            Genre: "Action",
            Status: AnimeStatus.Airing,
            SortBy: "title",
            SortOrder: "asc"
        ), CancellationToken.None);

        Assert.NotNull(capturedFilter);
        Assert.Equal("naruto", capturedFilter.Search);
        Assert.Equal("Action", capturedFilter.Genre);
        Assert.Equal(AnimeStatus.Airing, capturedFilter.Status);
        Assert.Equal("title", capturedFilter.SortBy);
        Assert.Equal("asc", capturedFilter.SortOrder);
    }

    private static AnimeListItem CreateAnimeListItem(string title = "Test Anime")
    {
        return new AnimeListItem(
            Guid.NewGuid(),
            Description.Create(title, 500),
            Description.Create("Original", 500),
            null,
            Synopsis.Create("Description"),
            ReleaseYear.Create(2024),
            AnimeStatus.Airing,
            null,
            null,
            null,
            AgeRating.Default,
            Rating.Create(0.0),
            0,
            0,
            true,
            DateTime.UtcNow,
            null,
            new[] { "Action" },
            new[] { "Studio" });
    }
}
