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
    IAnimeRepository animeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSeasonCommand>
{
    public async Task Handle(DeleteSeasonCommand request, CancellationToken ct)
    {
        var season = await seasonRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(request.Id));

        var anime = await animeRepository.GetByIdAsync(season.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(season.AnimeId));

        anime.RemoveSeason(season.Id);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
