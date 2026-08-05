using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ReleaseYear
{
    public static readonly int MinValue = 1900;
    public static int MaxValue => DateTime.UtcNow.Year + 5;

    public int Value { get; init; }

    private ReleaseYear(int value) => Value = value;

    public static ReleaseYear Create(int year)
    {
        if (year < MinValue || year > MaxValue)
            throw new EntityValidationException(ReleaseYearErrors.ReleaseYearOutOfRange(MinValue, MaxValue));

        return new ReleaseYear(year);
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(ReleaseYear year) => year.Value;
}
