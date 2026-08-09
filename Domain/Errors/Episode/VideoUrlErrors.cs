using Domain.Exceptions;

namespace Domain.Errors;

public static class VideoUrlErrors
{
    public static Error VideoUrlIsEmpty() => new(
        "VideoUrl.Empty", "Video URL cannot be empty.");

    public static Error VideoUrlTooLong(int maxLength) => new(
        "VideoUrl.TooLong", $"Video URL cannot exceed {maxLength} characters.");

    public static Error VideoUrlHasInvalidFormat() => new(
        "VideoUrl.InvalidFormat", "Video URL must be a valid absolute URL with http or https scheme.");
}
