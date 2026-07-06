using Application.Abstractions;
using Domain.Associations;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.UserAnime;

public record UpdateWatchlistCommand(
    Guid AnimeId,
    WatchStatus? Status,
    int? LastWatchedEpisodeNumber,
    double? ProgressPercentage,
    double? UserRating,
    bool? IsFavorite,
    string? Notes) : IRequest;

public class UpdateWatchlistCommandValidator : AbstractValidator<UpdateWatchlistCommand>
{
    public UpdateWatchlistCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status!.Value).IsInEnum();
        });

        When(x => x.LastWatchedEpisodeNumber.HasValue, () =>
        {
            RuleFor(x => x.LastWatchedEpisodeNumber!.Value)
                .GreaterThan(0).WithMessage("Episode number must be greater than 0.");
        });

        When(x => x.ProgressPercentage.HasValue, () =>
        {
            RuleFor(x => x.ProgressPercentage!.Value)
                .InclusiveBetween(0, 100);
        });

        When(x => x.UserRating.HasValue, () =>
        {
            RuleFor(x => x.UserRating!.Value)
                .InclusiveBetween(1, 10).WithMessage("Rating must be between 1.0 and 10.0.");
        });
    }
}

public class UpdateWatchlistHandler(
    ICurrentUser currentUser,
    IUserAnimeRepository userAnimeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWatchlistCommand>
{
    public async Task Handle(UpdateWatchlistCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var userAnime = await userAnimeRepository.GetByUserAndAnimeAsync(userId, request.AnimeId, ct)
            ?? throw new NotFoundException("Anime not found in your watchlist.");

        if (request.Status.HasValue)
            userAnime.UpdateStatus(request.Status.Value);

        if (request.LastWatchedEpisodeNumber.HasValue || request.ProgressPercentage.HasValue)
        {
            var ep = request.LastWatchedEpisodeNumber ?? userAnime.LastWatchedEpisodeNumber ?? 1;
            var prog = request.ProgressPercentage ?? userAnime.ProgressPercentage ?? 0;
            userAnime.UpdateProgress(ep, prog);
        }

        if (request.UserRating.HasValue)
            userAnime.Rate(request.UserRating.Value);

        if (request.IsFavorite.HasValue && request.IsFavorite != userAnime.IsFavorite)
            userAnime.ToggleFavorite();

        if (request.Notes is not null)
            userAnime.AddNote(request.Notes);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
