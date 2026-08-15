using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Errors;

public static class SeasonErrors
{
    public static Error SeasonNotFound(Guid id) => new(
        "SeasonNotFound",
        $"Season with ID '{id}' was not found.");

    public static Error SeasonNotFoundByNumber(int seasonNumber, Guid animeId) => new(
        "SeasonNotFoundByNumber",
        $"Season number {seasonNumber} was not found for anime '{animeId}'.");

    public static Error EndDateBeforeStartDate() => new(
        "EndDateBeforeStartDate",
        "End date cannot be before start date.");

    public static Error EpisodeNumberAlreadyExists(int episodeNumber, Guid seasonId) => new(
        "EpisodeNumberAlreadyExists",
        $"Episode number {episodeNumber} already exists in season '{seasonId}'.");

    public static Error DuplicateSeasonNumber(SeasonNumber seasonNumber) =>
    new("Season.DuplicateNumber", $"Season number {seasonNumber.Value} already exists for this anime.");
}
