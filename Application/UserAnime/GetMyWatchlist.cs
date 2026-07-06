using Application.Abstractions;
using Domain.Associations;
using Domain.Exceptions;
using MediatR;

namespace Application.UserAnime;

public record GetMyWatchlistQuery : IRequest<IReadOnlyList<WatchlistItemResponse>>;

public record WatchlistItemResponse(
    Guid AnimeId,
    string AnimeTitle,
    string? CoverImageUrl,
    WatchStatus Status,
    int? LastWatchedEpisodeNumber,
    double? ProgressPercentage,
    double? UserRating,
    bool IsFavorite,
    string? Notes,
    DateTime CreatedOnUtc,
    DateTime LastUpdatedOnUtc);

public class GetMyWatchlistHandler(
    ICurrentUser currentUser,
    IUserAnimeRepository userAnimeRepository)
    : IRequestHandler<GetMyWatchlistQuery, IReadOnlyList<WatchlistItemResponse>>
{
    public async Task<IReadOnlyList<WatchlistItemResponse>> Handle(GetMyWatchlistQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not { } userId)
            throw new ForbiddenException("User is not authenticated.");

        var watchlist = await userAnimeRepository.GetByUserIdAsync(userId, ct);

        return watchlist
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
