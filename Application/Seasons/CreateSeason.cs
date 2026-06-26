using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Seasons;

public record CreateSeasonCommand(
    Guid AnimeId,
    int SeasonNumber,
    string Title,
    string Description) : IRequest<SeasonResponse>;

public record SeasonResponse(
    Guid Id,
    Guid AnimeId,
    int SeasonNumber,
    string Title,
    string Description,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int EpisodesCount);

public class CreateSeasonCommandValidator : AbstractValidator<CreateSeasonCommand>
{
    public CreateSeasonCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty().WithMessage("Anime ID is required.");

        RuleFor(x => x.SeasonNumber)
            .GreaterThan(0).WithMessage("Season number must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Season title cannot be empty.")
            .MaximumLength(500).WithMessage("Season title must be at most 500 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Season description cannot be empty.")
            .MaximumLength(5000).WithMessage("Season description must be at most 5000 characters.");
    }
}

public class CreateSeasonHandler(
    IAnimeRepository animeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSeasonCommand, SeasonResponse>
{
    public async Task<SeasonResponse> Handle(CreateSeasonCommand request, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(request.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.AnimeId));

        var season = anime.AddSeason(request.SeasonNumber, request.Title, request.Description);

        await unitOfWork.SaveChangesAsync(ct);

        return new SeasonResponse(
            season.Id,
            season.AnimeId,
            season.SeasonNumber,
            season.Title,
            season.Description,
            season.StartDate,
            season.EndDate,
            season.EpisodesCount);
    }
}
