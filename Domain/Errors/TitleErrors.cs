using Domain.Exceptions;

namespace Domain.Errors;

public static class TitleErrors
{
    public static Error TitleIsEmpty() =>
        new Error("Title", "Title cannot be empty.");

    public static Error TitleTooLong(int maxLength) =>
        new Error("Title", $"Title cannot be longer than {maxLength} characters.");
}
