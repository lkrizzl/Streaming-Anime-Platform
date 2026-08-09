using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Description
{
    public string Value { get; init; }

    private Description(string value) => Value = value;

    public static Description Create(string? text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException(DescriptionErrors.DescriptionIsEmpty());

        string trimmed = text.Trim();

        if (trimmed.Length > maxLength)
            throw new ValidationException(DescriptionErrors.DescriptionTooLong(maxLength));

        return new Description(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Description d) => d.Value;
}