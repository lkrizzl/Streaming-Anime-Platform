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
            .Select(anime => new AnimeResponse(
                anime.Id,
                anime.Title,
                anime.OriginalTitle,
                anime.EnglishTitle?.Value,
                anime.Description,
                anime.ReleaseYear,
                anime.Status,
                anime.CoverImageUrl?.Value,
                anime.BannerImageUrl?.Value,
                anime.TrailerUrl?.Value,
                anime.AgeRating,
                anime.AverageRating,
                anime.RatingCount,
                anime.EpisodesCount,
                anime.IsActive,
                anime.CreatedOnUtc,
                anime.UpdatedOnUtc,
                anime.Genres.Select(g => g.Name.Value).ToList(),
                anime.Studios.Select(s => s.Name.Value).ToList()
            ))
            .ToList();

        return new PaginatedList<AnimeResponse>(items, paginated.Page, paginated.PageSize, paginated.TotalCount);
    }
}