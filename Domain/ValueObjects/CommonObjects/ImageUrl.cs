using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ImageUrl
{
    public const int MaxLength = 2048;

    public string Value { get; init; }

    private ImageUrl(string value) => Value = value;

    public static ImageUrl Create(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ValidationException(ImageUrlErrors.ImageUrlIsEmpty());

        string trimmed = url.Trim();

        if (trimmed.Length > MaxLength)
            throw new ValidationException(ImageUrlErrors.ImageUrlTooLong(MaxLength));

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
            || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new ValidationException(ImageUrlErrors.ImageUrlHasInvalidFormat());

        return new ImageUrl(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(ImageUrl url) => url.Value;
}
