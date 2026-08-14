using Application.Abstractions;
using Domain.ValueObjects;
using MediatR;

namespace Application.Animes;

public class RecalculateAnimeRatingHandler(
    IAnimeRepository animeRepository,
    IUserAnimeRepository userAnimeRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<AnimeRatedNotification>
{
    public async Task Handle(AnimeRatedNotification notification, CancellationToken ct)
    {
        var anime = await animeRepository.GetByIdAsync(notification.Event.AnimeId, ct);
        if (anime is null)
            return;

        var allRatings = await userAnimeRepository.GetByAnimeIdAsync(notification.Event.AnimeId, ct);
        var ratedEntries = allRatings.Where(r => r.UserRating is not null).ToList();

        var newCount = ratedEntries.Count;
        var newAverage = newCount > 0
            ? ratedEntries.Average(r => r.UserRating!.Value)
            : 0.0;

        anime.UpdateRating(Rating.Create(newAverage), newCount);

        await unitOfWork.SaveChangesAsync(ct);
    }
}