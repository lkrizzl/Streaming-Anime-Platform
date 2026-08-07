using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Title
{
    public const int MaxLength = 500;

    public string Value { get; init; }

    private Title(string value) => Value = value;

    public static Title Create(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException(TitleErrors.TitleIsEmpty());

        string trimmed = title.Trim();

        if (trimmed.Length > MaxLength)
            throw new ValidationException(TitleErrors.TitleTooLong(MaxLength));

        return new Title(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Title title) => title.Value;
}
