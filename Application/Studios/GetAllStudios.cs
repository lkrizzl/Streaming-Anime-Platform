using Application.Abstractions;
using MediatR;

namespace Application.Studios;

public record GetAllStudiosQuery(int Page = 1, int PageSize = 50) : IRequest<PaginatedList<StudioResponse>>;

public class GetAllStudiosHandler(IStudioRepository studioRepository)
    : IRequestHandler<GetAllStudiosQuery, PaginatedList<StudioResponse>>
{
    public async Task<PaginatedList<StudioResponse>> Handle(GetAllStudiosQuery request, CancellationToken ct)
    {
        var paginated = await studioRepository.GetAllAsync(request.Page, request.PageSize, ct);

        var items = paginated.Items
            .Select(s => new StudioResponse(s.Id, s.Name, s.Description))
            .ToList();

        return new PaginatedList<StudioResponse>(items, paginated.Page, paginated.PageSize, paginated.TotalCount);
    }
}
