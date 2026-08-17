using Application.Abstractions;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.UserAnime;

public record RemoveFromFavoritesCommand(Guid AnimeId) : IRequest;

public class RemoveFromFavoritesCommandValidator : AbstractValidator<RemoveFromFavoritesCommand>
{
    public RemoveFromFavoritesCommandValidator()
    {
        RuleFor(x => x.AnimeId).NotEmpty();
    }
}

public class RemoveFromFavoritesHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveFromFavoritesCommand>
{
    public async Task Handle(RemoveFromFavoritesCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var user = await userRepository.GetUserWithWatchlistAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        var userAnime = user.UserAnimes.FirstOrDefault(ua => ua.AnimeId == request.AnimeId)
            ?? throw new NotFoundException("Anime not found in your favorites.");

        userAnime.RemoveFavorite();

        await unitOfWork.SaveChangesAsync(ct);
    }
}