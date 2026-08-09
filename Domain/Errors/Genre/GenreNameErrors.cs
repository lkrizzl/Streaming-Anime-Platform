using Domain.Exceptions;

namespace Domain.Errors;

public static class GenreNameErrors
{
    public static Error GenreNameIsEmpty() =>
        new Error("GenreName", "Genre name cannot be empty.");

    public static Error GenreNameTooLong(int maxLength) =>
        new Error("GenreName", $"Genre name cannot be longer than {maxLength} characters.");
}
