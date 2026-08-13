using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.UserAnime;

public record AddToWatchlistCommand(Guid AnimeId, WatchStatus Status) : IRequest;

public class AddToWatchlistCommandValidator : AbstractValidator<AddToWatchlistCommand>
{
    public AddToWatchlistCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class AddToWatchlistHandler(
    ICurrentUser currentUser,
    IAnimeRepository animeRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddToWatchlistCommand>
{
    public async Task Handle(AddToWatchlistCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var anime = await animeRepository.GetByIdAsync(request.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.AnimeId));

        var user = await userRepository.GetUserWithWatchlistAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.UserAnimes.Any(ua => ua.AnimeId == request.AnimeId))
            throw new BadRequestException("Anime is already in your watchlist.");

        user.AddToWatchlist(request.AnimeId, request.Status);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
