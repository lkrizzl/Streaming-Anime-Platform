using Domain.Exceptions;

namespace Domain.Errors;

public static class AnimeErrors
{
    public static Error AnimeNotFound(Guid id) => new(
        "AnimeNotFound",
        $"Anime with ID '{id}' was not found.");

    public static Error GenreNotFoundInSystem(string name) => new(
        "GenreNotFoundInSystem",
        $"Genre '{name}' was not found in the system.");

    public static Error StudioNotFoundInSystem(string name) => new(
        "StudioNotFoundInSystem",
        $"Studio '{name}' was not found in the system.");
}
