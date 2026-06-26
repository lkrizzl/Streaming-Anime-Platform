using Application.Abstractions;
using MediatR;

namespace Application.Studios;

public record GetAllStudiosQuery : IRequest<IReadOnlyList<StudioResponse>>;

public class GetAllStudiosHandler(IStudioRepository studioRepository)
    : IRequestHandler<GetAllStudiosQuery, IReadOnlyList<StudioResponse>>
{
    public async Task<IReadOnlyList<StudioResponse>> Handle(GetAllStudiosQuery request, CancellationToken ct)
    {
        var studios = await studioRepository.GetAllAsync(ct);

        return studios
            .Select(s => new StudioResponse(s.Id, s.Name, s.Description))
            .ToList();
    }
}
