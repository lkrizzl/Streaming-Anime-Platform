using Domain.Exceptions;

namespace Domain.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFoundByEmailOrUsername = new(
        "UserNotFound",
        $"User with given email or username was not found."
    );

    public static readonly Error UserAlreadyExists = new(
        "UserAlreadyExists",
        $"User with given email or username already exists."
    );

    public static readonly Error WrongPassword = new(
        "WrongPassword",
        $"The password is wrong."
    );

    public static readonly Error InvalidCredentials = new(
        "InvalidCredentials",
        $"Invalid email, username, or password."
    );

    public static Error UserNotFound(Guid id) => new(
        "UserNotFound",
        $"User with ID '{id}' was not found."
    );

    public static readonly Error InvalidPassword = new(
        "InvalidPassword",
        $"Password is invalid."
    );
}
