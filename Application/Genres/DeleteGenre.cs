using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Genres;

public record DeleteGenreCommand(Guid Id) : IRequest;
public class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
{
    public DeleteGenreCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage("Genre ID is required.");
    }
}

public class DeleteGenreHandler(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteGenreCommand>
{
    public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(GenreErrors.GenreNotFound(request.Id));

        await genreRepository.DeleteAsync(genre, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
