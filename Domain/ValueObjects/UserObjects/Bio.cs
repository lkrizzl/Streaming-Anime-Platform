using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects.UserObjects;

public record Bio
{
    public const int MaxLength = 1000;
    public string Value { get; init; }
    private Bio(string value) => Value = value;

    public static Bio? Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        string trimmed = text.Trim();
        if (trimmed.Length > MaxLength)
            throw new ValidationException(BioErrors.BioTooLong(MaxLength));
        return new Bio(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Bio bio) => bio.Value;
}
