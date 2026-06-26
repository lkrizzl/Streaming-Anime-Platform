using Application.Abstractions;
using MediatR;

namespace Application.Animes;

public record GetAllAnimesQuery : IRequest<IReadOnlyList<AnimeResponse>>;

public class GetAllAnimesHandler(IAnimeRepository animeRepository)
    : IRequestHandler<GetAllAnimesQuery, IReadOnlyList<AnimeResponse>>
{
    public async Task<IReadOnlyList<AnimeResponse>> Handle(GetAllAnimesQuery request, CancellationToken ct)
    {
        var animes = await animeRepository.GetAllAsync(ct);

        return animes
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
    }
}
