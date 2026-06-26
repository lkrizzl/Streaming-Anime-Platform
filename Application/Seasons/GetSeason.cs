using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Seasons;

public record GetSeasonQuery(Guid Id) : IRequest<SeasonResponse>;

public class GetSeasonHandler(ISeasonRepository seasonRepository)
    : IRequestHandler<GetSeasonQuery, SeasonResponse>
{
    public async Task<SeasonResponse> Handle(GetSeasonQuery request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.Id));

        return new SeasonResponse(
            season.Id,
            season.AnimeId,
            season.SeasonNumber,
            season.Title,
            season.Description,
            season.StartDate,
            season.EndDate,
            season.EpisodesCount);
    }
}
