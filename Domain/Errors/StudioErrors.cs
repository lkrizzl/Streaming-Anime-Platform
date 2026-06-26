using Domain.Exceptions;

namespace Domain.Errors;

public static class StudioErrors
{
    public static Error StudioNotFound(Guid id) => new(
        "StudioNotFound",
        $"Studio with ID '{id}' was not found.");
}
