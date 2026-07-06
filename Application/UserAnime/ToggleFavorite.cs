using Application.Abstractions;
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
    IUserAnimeRepository userAnimeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleFavoriteCommand>
{
    public async Task Handle(ToggleFavoriteCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var userAnime = await userAnimeRepository.GetByUserAndAnimeAsync(userId, request.AnimeId, ct);

        if (userAnime is null)
        {
            userAnime = new Domain.Associations.UserAnime(
                userId, request.AnimeId, Domain.Associations.WatchStatus.Planned);
            await userAnimeRepository.AddAsync(userAnime, ct);
        }

        userAnime.ToggleFavorite();
        await unitOfWork.SaveChangesAsync(ct);
    }
}
