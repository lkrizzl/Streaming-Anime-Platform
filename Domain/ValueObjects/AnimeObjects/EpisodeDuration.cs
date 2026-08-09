using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects.AnimeObjects;

public record EpisodeDuration
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(6);
    public TimeSpan Value { get; init; }
    private EpisodeDuration(TimeSpan value) => Value = value;

    public static EpisodeDuration Create(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ValidationException(EpisodeErrors.DurationMustBePositive());
        if (duration > MaxDuration)
            throw new ValidationException(EpisodeErrors.DurationTooLong(MaxDuration));

        return new EpisodeDuration(duration);
    }

    public override string ToString() => Value.ToString();
    public static implicit operator TimeSpan(EpisodeDuration d) => d.Value;
}