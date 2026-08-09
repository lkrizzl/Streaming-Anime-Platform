using Domain.Exceptions;

namespace Domain.Errors;

public static class ProgressPercentErrors
{
    public static Error ProgressPercentOutOfRange(double min, double max) =>
        new Error("ProgressPercent", $"Progress percentage must be between {min} and {max}.");
}
