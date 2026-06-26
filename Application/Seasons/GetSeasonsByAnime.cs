using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Seasons;

public record GetSeasonsByAnimeQuery(Guid AnimeId) : IRequest<IReadOnlyList<SeasonResponse>>;

public class GetSeasonsByAnimeHandler(
    ISeasonRepository seasonRepository,
    IAnimeRepository animeRepository)
    : IRequestHandler<GetSeasonsByAnimeQuery, IReadOnlyList<SeasonResponse>>
{
    public async Task<IReadOnlyList<SeasonResponse>> Handle(GetSeasonsByAnimeQuery request, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(request.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.AnimeId));

        var seasons = await seasonRepository.GetByAnimeIdAsync(request.AnimeId, ct);

        return seasons
            .Select(s => new SeasonResponse(
                s.Id,
                s.AnimeId,
                s.SeasonNumber,
                s.Title,
                s.Description,
                s.StartDate,
                s.EndDate,
                s.EpisodesCount))
            .ToList();
    }
}
