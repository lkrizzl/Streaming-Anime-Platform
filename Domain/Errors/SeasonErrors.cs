using Domain.Exceptions;

namespace Domain.Errors;

public static class SeasonErrors
{
    public static Error SeasonNotFound(Guid id) => new(
        "SeasonNotFound",
        $"Season with ID '{id}' was not found.");

    public static Error SeasonNotFoundByNumber(int seasonNumber, Guid animeId) => new(
        "SeasonNotFoundByNumber",
        $"Season number {seasonNumber} was not found for anime '{animeId}'.");
}
