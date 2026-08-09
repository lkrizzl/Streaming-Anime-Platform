using Domain.Exceptions;

namespace Domain.Errors;

public static class PasswordErrors
{
    public static Error PasswordIsEmpty() =>
        new Error("Password", "Password cannot be empty.");

    public static Error PasswordTooShort(int minLength) =>
        new Error("Password", $"Password must be at least {minLength} characters long.");

    public static Error PasswordTooLong(int maxLength) =>
        new Error("Password", $"Password cannot be longer than {maxLength} characters.");

    public static Error PasswordHasInvalidFormat() =>
        new Error("Password", "Password must contain at least one letter and one number.");
}
