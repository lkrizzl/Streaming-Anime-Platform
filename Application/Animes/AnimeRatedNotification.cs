using Domain.Events;
using MediatR;


namespace Application.Animes;

public record AnimeRatedNotification(AnimeRatedEvent Event) : INotification;

