using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Synopsis
{
    public const int MaxLength = 5000;

    public string Value { get; init; }

    private Synopsis(string value) => Value = value;

    public static Synopsis Create(string? text, int maxLength = MaxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException(SynopsisErrors.SynopsisIsEmpty());

        var trimmed = text.Trim();

        if (trimmed.Length > maxLength)
            throw new ValidationException(SynopsisErrors.SynopsisTooLong(maxLength));

        return new Synopsis(trimmed);
    }

    public static Synopsis? CreateOptional(string? text, int maxLength = MaxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return Create(text, maxLength);
    }

    public override string ToString() => Value;
    public static implicit operator string(Synopsis s) => s.Value;
}