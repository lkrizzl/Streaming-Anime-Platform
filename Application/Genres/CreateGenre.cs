using Application.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Genres;

// ====== Command ======

public record CreateGenreCommand(string Name, string? Description) : IRequest<GenreResponse>;

// ====== Response ======

public record GenreResponse(Guid Id, string Name, string? Description);

// ====== Validator ======

public class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    private readonly IGenreRepository _genreRepository;

    public CreateGenreCommandValidator(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;

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

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var existing = await _genreRepository.GetByNameAsync(name, cancellationToken);
        return existing is null;
    }
}

// ====== Handler ======

public class CreateGenreHandler(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGenreCommand, GenreResponse>
{
    public async Task<GenreResponse> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = new Genre(request.Name, request.Description);

        await genreRepository.AddAsync(genre, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GenreResponse(genre.Id, genre.Name, genre.Description);
    }
}
