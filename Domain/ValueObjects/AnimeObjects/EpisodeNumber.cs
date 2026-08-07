using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record EpisodeNumber
{
    public int Value { get; init; }

    private EpisodeNumber(int value) => Value = value;

    public static EpisodeNumber Create(int number)
    {
        if (number < 1)
            throw new ValidationException(EpisodeNumberErrors.EpisodeNumberMustBePositive());

        return new EpisodeNumber(number);
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(EpisodeNumber number) => number.Value;
}
