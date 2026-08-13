using Application.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.UserAnime;

public record ToggleFavoriteCommand(Guid AnimeId) : IRequest;

public class ToggleFavoriteCommandValidator : AbstractValidator<ToggleFavoriteCommand>
{
    public ToggleFavoriteCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();
    }
}

public class ToggleFavoriteHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleFavoriteCommand>
{
    public async Task Handle(ToggleFavoriteCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var user = await userRepository.GetUserWithWatchlistAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        var userAnime = user.UserAnimes.FirstOrDefault(ua => ua.AnimeId == request.AnimeId)
            ?? user.AddToWatchlist(request.AnimeId, WatchStatus.Planned);

        userAnime.ToggleFavorite();

        await unitOfWork.SaveChangesAsync(ct);
    }
}
