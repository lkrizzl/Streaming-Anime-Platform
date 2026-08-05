using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Animes;

public record GetAnimeQuery(Guid Id) : IRequest<AnimeResponse>;

public class GetAnimeHandler(IAnimeRepository animeRepository)
    : IRequestHandler<GetAnimeQuery, AnimeResponse>
{
    public async Task<AnimeResponse> Handle(GetAnimeQuery request, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.Id));

        return new AnimeResponse(
            anime.Id,
            anime.Title,
            anime.OriginalTitle,
            anime.EnglishTitle,
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
        );
    }
}
