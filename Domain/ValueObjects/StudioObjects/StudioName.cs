using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record StudioName
{
    public static readonly int MaxLength = 200;

    public string Value { get; init; }

    private StudioName(string value) => Value = value;

    public static StudioName Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EntityValidationException(StudioNameErrors.StudioNameIsEmpty());

        string trimmed = name.Trim();

        if (trimmed.Length > MaxLength)
            throw new EntityValidationException(StudioNameErrors.StudioNameTooLong(MaxLength));

        return new StudioName(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(StudioName name) => name.Value;
}
