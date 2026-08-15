using Domain.Exceptions;

namespace Domain.Errors;

public static class AgeRatingErrors
{
    public static Error InvalidAgeRating(string? value, IEnumerable<string> allowed) =>
        new("AgeRating.Invalid", $"'{value}' is not a valid age rating. Allowed values: {string.Join(", ", allowed)}.");
}