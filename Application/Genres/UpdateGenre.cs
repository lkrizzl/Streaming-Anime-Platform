using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Genres;

// ====== Command ======

public record UpdateGenreCommand(Guid Id, string Name, string? Description) : IRequest;

// ====== Validator ======

public class UpdateGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
{
    private readonly IGenreRepository _genreRepository;

    public UpdateGenreCommandValidator(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage("Genre ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Genre name cannot be empty.")
            .MaximumLength(100)
                .WithMessage("Genre name must be at most 100 characters.")
            .MustAsync(BeUniqueName)
                .WithMessage("Genre with this name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
                .WithMessage("Genre description must be at most 500 characters.");
    }

    private async Task<bool> BeUniqueName(UpdateGenreCommand command, string name, CancellationToken cancellationToken)
    {
        var existing = await _genreRepository.GetByNameAsync(name, cancellationToken);
        return existing is null || existing.Id == command.Id;
    }
}

// ====== Handler ======

public class UpdateGenreHandler(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateGenreCommand>
{
    public async Task Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(GenreErrors.GenreNotFound(request.Id));

        genre.UpdateName(request.Name);
        genre.UpdateDescription(request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
