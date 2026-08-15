using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record AgeRating
{
    private static readonly string[] AllowedValues = { "0+", "6+", "12+", "16+", "18+" };

    public string Value { get; init; }

    private AgeRating(string value) => Value = value;

    public static AgeRating Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !AllowedValues.Contains(value))
            throw new ValidationException(AgeRatingErrors.InvalidAgeRating(value, AllowedValues));

        return new AgeRating(value);
    }

    public static readonly AgeRating Default = new("16+");

    public override string ToString() => Value;
    public static implicit operator string(AgeRating a) => a.Value;
}