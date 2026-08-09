using Domain.Exceptions;

namespace Domain.Errors;

public static class RatingErrors
{
    public static Error RatingOutOfRange(double min, double max) =>
        new Error("Rating", $"Rating must be between {min} and {max}.");
}
