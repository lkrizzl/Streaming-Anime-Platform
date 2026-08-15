using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects.UserObjects;

public record Notes
{
    public const int MaxLength = 2000;

    public string Value { get; init; }

    private Notes(string value) => Value = value;

    public static Notes? Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var trimmed = text.Trim();

        if (trimmed.Length > MaxLength)
            throw new ValidationException(NotesErrors.NotesTooLong(MaxLength));

        return new Notes(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Notes n) => n.Value;
}