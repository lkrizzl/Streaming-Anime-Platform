using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects.AnimeObjects;

public record VideoUrl
{
    public const int MaxLength = 2048;
    public string Value { get; init; }
    private VideoUrl(string value) => Value = value;

    public static VideoUrl Create(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ValidationException(VideoUrlErrors.VideoUrlIsEmpty());

        string trimmed = url.Trim();
        if (trimmed.Length > MaxLength)
            throw new ValidationException(VideoUrlErrors.VideoUrlTooLong(MaxLength));

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
            || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new ValidationException(VideoUrlErrors.VideoUrlHasInvalidFormat());

        return new VideoUrl(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(VideoUrl url) => url.Value;
}
