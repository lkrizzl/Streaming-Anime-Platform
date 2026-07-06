using Application.Abstractions;
using Domain.Associations;
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
    IUserAnimeRepository userAnimeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddToWatchlistCommand>
{
    public async Task Handle(AddToWatchlistCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var anime = await animeRepository.GetByIdAsync(request.AnimeId, ct)
            ?? throw new NotFoundException(AnimeErrors.AnimeNotFound(request.AnimeId));

        var existing = await userAnimeRepository.GetByUserAndAnimeAsync(userId, request.AnimeId, ct);
        if (existing is not null)
            throw new BadRequestException("Anime is already in your watchlist.");

        var userAnime = new Domain.Associations.UserAnime(userId, request.AnimeId, request.Status);

        await userAnimeRepository.AddAsync(userAnime, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
