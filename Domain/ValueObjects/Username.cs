using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Username
{
    public static readonly int MinLength = 3;
    public static readonly int MaxLength = 30;

    private static readonly Regex UsernameRegex =
        new(@"^[A-Za-z0-9]+(?:[._-][A-Za-z0-9]+)*$", RegexOptions.Compiled);

    public string Value { get; init; }

    private Username(string value) => Value = value;

    public static Username Create(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new EntityValidationException(UsernameErrors.UsernameIsEmpty());

        string trimmed = username.Trim();

        if (trimmed.Length < MinLength)
            throw new EntityValidationException(UsernameErrors.UsernameTooShort(MinLength));

        if (trimmed.Length > MaxLength)
            throw new EntityValidationException(UsernameErrors.UsernameTooLong(MaxLength));

        if (!UsernameRegex.IsMatch(trimmed))
            throw new EntityValidationException(UsernameErrors.UsernameHasInvalidFormat());

        return new Username(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Username username) => username.Value;
}