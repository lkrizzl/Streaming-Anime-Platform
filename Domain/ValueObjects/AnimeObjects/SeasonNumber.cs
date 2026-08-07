using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record SeasonNumber
{
    public int Value { get; init; }

    private SeasonNumber(int value) => Value = value;

    public static SeasonNumber Create(int number)
    {
        if (number < 1)
            throw new ValidationException(SeasonNumberErrors.SeasonNumberMustBePositive());

        return new SeasonNumber(number);
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(SeasonNumber number) => number.Value;
}
