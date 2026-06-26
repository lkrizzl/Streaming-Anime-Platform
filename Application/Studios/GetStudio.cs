using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Studios;

public record GetStudioQuery(Guid Id) : IRequest<StudioResponse>;

public class GetStudioHandler(IStudioRepository studioRepository)
    : IRequestHandler<GetStudioQuery, StudioResponse>
{
    public async Task<StudioResponse> Handle(GetStudioQuery request, CancellationToken ct)
    {
        var studio = await studioRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(StudioErrors.StudioNotFound(request.Id));

        return new StudioResponse(studio.Id, studio.Name, studio.Description);
    }
}
