using Domain.Exceptions;

namespace Domain.Errors;

public static class SynopsisErrors
{
    public static Error SynopsisIsEmpty() =>
        new("Synopsis.Empty", "Synopsis cannot be empty.");

    public static Error SynopsisTooLong(int maxLength) =>
        new("Synopsis.TooLong", $"Synopsis must be at most {maxLength} characters.");
}