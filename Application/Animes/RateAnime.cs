using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Application.Animes;

public record RateAnimeCommand(Guid AnimeId, double Rating) : IRequest;

public class RateAnimeCommandValidator : AbstractValidator<RateAnimeCommand>
{
    public RateAnimeCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();
        RuleFor(x => x.Rating)
            .InclusiveBetween(1.0, 10.0)
            .WithMessage("Rating must be between 1.0 and 10.0.");
    }
}

public class RateAnimeHandler(
    ICurrentUser currentUser,
    IAnimeRepository animeRepository,
    IUserRepository userRepository,
    IUserAnimeRepository userAnimeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RateAnimeCommand>
{
    public async Task Handle(RateAnimeCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var anime = await animeRepository.GetByIdAsync(request.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.AnimeId));

        var userAnime = await userAnimeRepository.GetByUserAndAnimeAsync(userId, request.AnimeId, ct);
        if (userAnime is null)
        {
            var user = await userRepository.GetUserWithWatchlistAsync(userId, ct)
                ?? throw new NotFoundException("User not found.");

            userAnime = user.AddToWatchlist(request.AnimeId, WatchStatus.Planned);
        }

        userAnime.Rate(Rating.Create(request.Rating));

        var allRatings = await userAnimeRepository.GetByAnimeIdAsync(request.AnimeId, ct);
        var newCount = allRatings.Count(r => r.UserRating is not null);
        var newAverage = newCount > 0
            ? allRatings.Where(r => r.UserRating is not null).Average(r => r.UserRating!.Value)
            : 0.0;

        anime.UpdateRating(Rating.Create(newAverage), newCount);

        await unitOfWork.SaveChangesAsync(ct);
    }
}