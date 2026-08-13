using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Episodes;

public record DeleteEpisodeCommand(Guid Id) : IRequest;

public class DeleteEpisodeCommandValidator : AbstractValidator<DeleteEpisodeCommand>
{
    public DeleteEpisodeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Episode ID is required.");
    }
}

public class DeleteEpisodeHandler(
    IEpisodeRepository episodeRepository,
    ISeasonRepository seasonRepository,
    IAnimeRepository animeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEpisodeCommand>
{
    public async Task Handle(DeleteEpisodeCommand request, CancellationToken ct)
    {
        var episode = await episodeRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(EpisodeErrors.EpisodeNotFound(request.Id));

        var season = await seasonRepository.GetByIdAsync(episode.SeasonId, ct)
            ?? throw new NotFoundException(SeasonErrors.SeasonNotFound(episode.SeasonId));

        var anime = await animeRepository.GetByIdAsync(season.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(season.AnimeId));

        var targetSeason = anime.Seasons.First(s => s.Id == season.Id);
        targetSeason.RemoveEpisode(episode.Id);

        await unitOfWork.SaveChangesAsync(ct);
    }
}