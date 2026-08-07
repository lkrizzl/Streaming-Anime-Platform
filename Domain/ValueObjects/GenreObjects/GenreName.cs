using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record GenreName
{
    public const int MaxLength = 100;

    public string Value { get; init; }

    private GenreName(string value) => Value = value;

    public static GenreName Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException(GenreNameErrors.GenreNameIsEmpty());

        string trimmed = name.Trim();

        if (trimmed.Length > MaxLength)
            throw new ValidationException(GenreNameErrors.GenreNameTooLong(MaxLength));

        return new GenreName(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(GenreName name) => name.Value;
}
