using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Episodes;

public record GetEpisodeQuery(Guid Id) : IRequest<EpisodeResponse>;

public class GetEpisodeHandler(IEpisodeRepository episodeRepository)
    : IRequestHandler<GetEpisodeQuery, EpisodeResponse>
{
    public async Task<EpisodeResponse> Handle(GetEpisodeQuery request, CancellationToken ct)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        return new EpisodeResponse(
            episode.Id,
            episode.SeasonId,
            episode.EpisodeNumber,
            episode.Title,
            episode.Description,
            episode.Duration,
            episode.VideoUrl,
            episode.ThumbnailUrl,
            episode.ReleaseDateUtc,
            episode.IsPublished);
    }
}
