using Application.Abstractions;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.UserAnime;

public record RemoveFromWatchlistCommand(Guid AnimeId) : IRequest;

public class RemoveFromWatchlistCommandValidator : AbstractValidator<RemoveFromWatchlistCommand>
{
    public RemoveFromWatchlistCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();
    }
}

public class RemoveFromWatchlistHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveFromWatchlistCommand>
{
    public async Task Handle(RemoveFromWatchlistCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var user = await userRepository.GetUserWithWatchlistAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        var removed = user.RemoveFromWatchlist(request.AnimeId)
            ?? throw new NotFoundException("Anime not found in your watchlist.");

        await unitOfWork.SaveChangesAsync(ct);
    }
}
