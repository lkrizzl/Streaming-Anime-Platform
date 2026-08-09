using Domain.Exceptions;

namespace Domain.Errors;

public static class EmailErrors
{
    public static Error EmailIsEmpty() =>
        new Error("Email", "Email cannot be empty.");

    public static Error EmailTooLong(int maxLength) =>
        new Error("Email", $"Email cannot be longer than {maxLength} characters.");

    public static Error EmailHasInvalidFormat() =>
        new Error("Email", "Email has an invalid format.");
}
