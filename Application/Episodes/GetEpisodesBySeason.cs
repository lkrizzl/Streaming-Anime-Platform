using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Episodes;

public record GetEpisodesBySeasonQuery(Guid SeasonId) : IRequest<IReadOnlyList<EpisodeResponse>>;

public class GetEpisodesBySeasonHandler(
    IEpisodeRepository episodeRepository,
    ISeasonRepository seasonRepository)
    : IRequestHandler<GetEpisodesBySeasonQuery, IReadOnlyList<EpisodeResponse>>
{
    public async Task<IReadOnlyList<EpisodeResponse>> Handle(GetEpisodesBySeasonQuery request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.SeasonId, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.SeasonId));

        var episodes = await episodeRepository.GetBySeasonIdAsync(request.SeasonId, ct);

        return episodes
            .Select(e => new EpisodeResponse(
                e.Id,
                e.SeasonId,
                e.EpisodeNumber,
                e.Title,
                e.Description,
                e.Duration,
                e.VideoUrl,
                e.ThumbnailUrl,
                e.ReleaseDateUtc,
                e.IsPublished))
            .ToList();
    }
}
