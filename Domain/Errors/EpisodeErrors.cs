using Domain.Exceptions;

namespace Domain.Errors;

public static class EpisodeErrors
{
    public static Error EpisodeNotFound(Guid id) => new(
        "EpisodeNotFound",
        $"Episode with ID '{id}' was not found.");

    public static Error EpisodeNotFoundByNumber(int episodeNumber, Guid seasonId) => new(
        "EpisodeNotFoundByNumber",
        $"Episode number {episodeNumber} was not found in season '{seasonId}'.");

    public static readonly Error EpisodeAlreadyPublished = new(
        "EpisodeAlreadyPublished",
        "Episode is already published.");
}
