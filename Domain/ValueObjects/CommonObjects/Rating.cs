using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Rating
{
    public const double MinValue = 0.0;
    public const double MaxValue = 10.0;

    public double Value { get; init; }

    private Rating(double value) => Value = value;

    public static Rating Create(double rating)
    {
        if (rating < MinValue || rating > MaxValue)
            throw new ValidationException(RatingErrors.RatingOutOfRange(MinValue, MaxValue));

        return new Rating(Math.Round(rating, 1));
    }

    public override string ToString() => Value.ToString("F1");
    public static implicit operator double(Rating rating) => rating.Value;
}
