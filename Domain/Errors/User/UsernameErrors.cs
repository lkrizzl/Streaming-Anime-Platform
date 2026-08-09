using Domain.Exceptions;

namespace Domain.Errors;

public static class UsernameErrors
{
    public static Error UsernameIsEmpty() =>
        new Error("Username", "Username cannot be empty.");

    public static Error UsernameTooShort(int minLength) =>
        new Error("Username", $"Username must be at least {minLength} characters long.");

    public static Error UsernameTooLong(int maxLength) =>
        new Error("Username", $"Username cannot be longer than {maxLength} characters.");

    public static Error UsernameHasInvalidFormat() =>
        new Error("Username", "Username has an invalid format.");
}
