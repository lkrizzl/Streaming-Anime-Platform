using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Studios;

public record UpdateStudioCommand(Guid Id, string Name, string? Description) : IRequest;

public class UpdateStudioCommandValidator : AbstractValidator<UpdateStudioCommand>
{
    private readonly IStudioRepository _studioRepository;

    public UpdateStudioCommandValidator(IStudioRepository studioRepository)
    {
        _studioRepository = studioRepository;

        RuleFor(x => x.Id).NotEmpty().WithMessage("Studio ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Studio name cannot be empty.")
            .MaximumLength(200).WithMessage("Studio name must be at most 200 characters.")
            .MustAsync(BeUniqueName).WithMessage("Studio with this name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Studio description must be at most 2000 characters.");
    }

    private async Task<bool> BeUniqueName(UpdateStudioCommand command, string name, CancellationToken ct)
    {
        var existing = await _studioRepository.GetByNameAsync(name, ct);
        return existing is null || existing.Id == command.Id;
    }
}

public class UpdateStudioHandler(IStudioRepository studioRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateStudioCommand>
{
    public async Task Handle(UpdateStudioCommand request, CancellationToken ct)
    {
        var studio = await studioRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(StudioErrors.StudioNotFound(request.Id));

        studio.UpdateName(request.Name);
        studio.UpdateDescription(request.Description);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
