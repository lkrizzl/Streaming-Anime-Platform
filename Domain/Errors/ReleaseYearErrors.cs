using Domain.Exceptions;

namespace Domain.Errors;

public static class ReleaseYearErrors
{
    public static Error ReleaseYearOutOfRange(int min, int max) =>
        new Error("ReleaseYear", $"Release year must be between {min} and {max}.");
}
