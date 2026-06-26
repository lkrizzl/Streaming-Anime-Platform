using Domain.Exceptions;

namespace Domain.Errors;

public static class GenreErrors
{
    public static Error GenreNotFound(Guid id) => new(
        "GenreNotFound",
        $"Genre with ID '{id}' was not found.");

    public static readonly Error GenreNotFoundByName = new(
        "GenreNotFoundByName",
        "Genre with given name was not found.");
}
