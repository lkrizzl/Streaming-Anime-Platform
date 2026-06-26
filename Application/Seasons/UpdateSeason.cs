using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Seasons;

public record UpdateSeasonCommand(
    Guid Id,
    string Title,
    string Description,
    DateOnly? StartDate,
    DateOnly? EndDate) : IRequest;

public class UpdateSeasonCommandValidator : AbstractValidator<UpdateSeasonCommand>
{
    public UpdateSeasonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Season ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Season title cannot be empty.")
            .MaximumLength(500).WithMessage("Season title must be at most 500 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Season description cannot be empty.")
            .MaximumLength(5000).WithMessage("Season description must be at most 5000 characters.");
    }
}

public class UpdateSeasonHandler(
    ISeasonRepository seasonRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSeasonCommand>
{
    public async Task Handle(UpdateSeasonCommand request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.Id));

        season.UpdateTitle(request.Title);
        season.UpdateDescription(request.Description);
        season.UpdateDates(request.StartDate, request.EndDate);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
