using Domain.Exceptions;

namespace Domain.Errors;

public static class StudioNameErrors
{
    public static Error StudioNameIsEmpty() =>
        new Error("StudioName", "Studio name cannot be empty.");

    public static Error StudioNameTooLong(int maxLength) =>
        new Error("StudioName", $"Studio name cannot be longer than {maxLength} characters.");
}
