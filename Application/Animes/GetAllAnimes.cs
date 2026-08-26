using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Animes;

public record GetAllAnimesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Genre = null,
    AnimeStatus? Status = null,
    int? ReleaseYear = null,
    string? Studio = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? SortBy = "created",
    string? SortOrder = "desc") : IRequest<PaginatedList<AnimeResponse>>;

public class GetAllAnimesHandler(IAnimeRepository animeRepository)
    : IRequestHandler<GetAllAnimesQuery, PaginatedList<AnimeResponse>>
{
    public async Task<PaginatedList<AnimeResponse>> Handle(GetAllAnimesQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var filter = new AnimeFilter(
            Search: request.Search,
            Genre: request.Genre,
            Status: request.Status,
            ReleaseYear: request.ReleaseYear,
            Studio: request.Studio,
            FromDate: request.FromDate,
            ToDate: request.ToDate,
            SortBy: request.SortBy,
            SortOrder: request.SortOrder);

        var paginated = await animeRepository.GetAllAsync(request.Page, pageSize, filter, ct);

        var items = paginated.Items
            .Select(a => new AnimeResponse(
                a.Id,
                a.Title,
                a.OriginalTitle,
                a.EnglishTitle?.Value,
                a.Description,
                a.ReleaseYear,
                a.Status,
                a.CoverImageUrl?.Value,
                a.BannerImageUrl?.Value,
                a.TrailerUrl?.Value,
                a.AgeRating,
                a.AverageRating,
                a.RatingCount,
                a.EpisodesCount,
                a.IsActive,
                a.CreatedOnUtc,
                a.UpdatedOnUtc,
                a.GenreNames.ToList(),
                a.StudioNames.ToList()
            ))
            .ToList();

        return new PaginatedList<AnimeResponse>(items, paginated.Page, paginated.PageSize, paginated.TotalCount);
    }
}