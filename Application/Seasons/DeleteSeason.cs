using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Seasons;

public record DeleteSeasonCommand(Guid Id) : IRequest;

public class DeleteSeasonCommandValidator : AbstractValidator<DeleteSeasonCommand>
{
    public DeleteSeasonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Season ID is required.");
    }
}

public class DeleteSeasonHandler(
    ISeasonRepository seasonRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSeasonCommand>
{
    public async Task Handle(DeleteSeasonCommand request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.Id));

        await seasonRepository.DeleteAsync(season, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
