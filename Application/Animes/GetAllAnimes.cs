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
    string? SortBy = "created",
    string? SortOrder = "desc") : IRequest<PaginatedList<AnimeResponse>>;

public class GetAllAnimesHandler(IAnimeRepository animeRepository)
    : IRequestHandler<GetAllAnimesQuery, PaginatedList<AnimeResponse>>
{
    public async Task<PaginatedList<AnimeResponse>> Handle(GetAllAnimesQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var filter = new AnimeFilter(
            request.Search,
            request.Genre,
            request.Status,
            request.SortBy,
            request.SortOrder);

        var paginated = await animeRepository.GetAllAsync(request.Page, pageSize, filter, ct);

        var items = paginated.Items
            .Select(anime => new AnimeResponse(
                anime.Id,
                anime.Title,
                anime.OriginalTitle,
                anime.EnglishTitle,
                anime.Description,
                anime.ReleaseYear,
                anime.Status,
                anime.CoverImageUrl,
                anime.BannerImageUrl,
                anime.TrailerUrl,
                anime.AgeRating,
                anime.AverageRating,
                anime.RatingCount,
                anime.EpisodesCount,
                anime.IsActive,
                anime.CreatedOnUtc,
                anime.UpdatedOnUtc,
                anime.Genres.Select(g => g.Name).ToList(),
                anime.Studios.Select(s => s.Name).ToList()
            ))
            .ToList();

        return new PaginatedList<AnimeResponse>(items, paginated.Page, paginated.PageSize, paginated.TotalCount);
    }
}
