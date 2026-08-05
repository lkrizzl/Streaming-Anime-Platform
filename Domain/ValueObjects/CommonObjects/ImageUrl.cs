using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ImageUrl
{
    public static readonly int MaxLength = 2048;

    public string Value { get; init; }

    private ImageUrl(string value) => Value = value;

    public static ImageUrl Create(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new EntityValidationException(ImageUrlErrors.ImageUrlIsEmpty());

        string trimmed = url.Trim();

        if (trimmed.Length > MaxLength)
            throw new EntityValidationException(ImageUrlErrors.ImageUrlTooLong(MaxLength));

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
            || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new EntityValidationException(ImageUrlErrors.ImageUrlHasInvalidFormat());

        return new ImageUrl(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(ImageUrl url) => url.Value;
}
