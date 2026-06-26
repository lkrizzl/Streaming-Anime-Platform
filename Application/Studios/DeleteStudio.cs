using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using MediatR;

namespace Application.Studios;

public record DeleteStudioCommand(Guid Id) : IRequest;

public class DeleteStudioHandler(IStudioRepository studioRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteStudioCommand>
{
    public async Task Handle(DeleteStudioCommand request, CancellationToken ct)
    {
        var studio = await studioRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(StudioErrors.StudioNotFound(request.Id));

        await studioRepository.DeleteAsync(studio, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
