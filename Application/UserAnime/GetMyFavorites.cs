using Application.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.UserAnime;

public record GetMyFavoritesQuery : IRequest<IReadOnlyList<WatchlistItemResponse>>;

public class GetMyFavoritesHandler(
    ICurrentUser currentUser,
    IUserAnimeRepository userAnimeRepository)
    : IRequestHandler<GetMyFavoritesQuery, IReadOnlyList<WatchlistItemResponse>>
{
    public async Task<IReadOnlyList<WatchlistItemResponse>> Handle(GetMyFavoritesQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var watchlist = await userAnimeRepository.GetByUserIdAsync(userId, ct);

        return watchlist
            .Where(ua => ua.IsFavorite)
            .Select(ua => new WatchlistItemResponse(
                ua.AnimeId,
                ua.Anime.Title,
                ua.Anime.CoverImageUrl,
                ua.Status,
                ua.LastWatchedEpisodeNumber,
                ua.ProgressPercentage,
                ua.UserRating,
                ua.IsFavorite,
                ua.Notes,
                ua.CreatedOnUtc,
                ua.LastUpdatedOnUtc))
            .ToList();
    }
}
