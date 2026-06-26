using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Studios;

public record CreateStudioCommand(string Name, string? Description) : IRequest<StudioResponse>;

public record StudioResponse(Guid Id, string Name, string? Description);

public class CreateStudioCommandValidator : AbstractValidator<CreateStudioCommand>
{
    private readonly IStudioRepository _studioRepository;

    public CreateStudioCommandValidator(IStudioRepository studioRepository)
    {
        _studioRepository = studioRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Studio name cannot be empty.")
            .MaximumLength(200).WithMessage("Studio name must be at most 200 characters.")
            .MustAsync(BeUniqueName).WithMessage("Studio with this name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Studio description must be at most 2000 characters.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken ct)
    {
        var existing = await _studioRepository.GetByNameAsync(name, ct);
        return existing is null;
    }
}

public class CreateStudioHandler(IStudioRepository studioRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateStudioCommand, StudioResponse>
{
    public async Task<StudioResponse> Handle(CreateStudioCommand request, CancellationToken ct)
    {
        var studio = new Studio(request.Name, request.Description);

        await studioRepository.AddAsync(studio, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new StudioResponse(studio.Id, studio.Name, studio.Description);
    }
}
