using Domain.Exceptions;

namespace Domain.Errors;

public static class ImageUrlErrors
{
    public static Error ImageUrlIsEmpty() =>
        new Error("ImageUrl", "Image URL cannot be empty.");

    public static Error ImageUrlTooLong(int maxLength) =>
        new Error("ImageUrl", $"Image URL cannot be longer than {maxLength} characters.");

    public static Error ImageUrlHasInvalidFormat() =>
        new Error("ImageUrl", "Image URL must be a valid HTTP or HTTPS URL.");
}
