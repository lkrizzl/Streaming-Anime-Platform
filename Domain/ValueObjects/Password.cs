using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Password
{
    public static readonly int MinLength = 8;
    public static readonly int MaxLength = 64;

    private static readonly Regex PasswordRegex =
        new(@"^(?=.*[A-Za-z])(?=.*\d).{8,64}$", RegexOptions.Compiled);

    public string Value { get; init; }

    private Password(string value) => Value = value;

    public static Password Create(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new EntityValidationException(PasswordErrors.PasswordIsEmpty());

        if (password.Length < MinLength)
            throw new EntityValidationException(PasswordErrors.PasswordTooShort(MinLength));

        if (password.Length > MaxLength)
            throw new EntityValidationException(PasswordErrors.PasswordTooLong(MaxLength));

        if (!PasswordRegex.IsMatch(password))
            throw new EntityValidationException(PasswordErrors.PasswordHasInvalidFormat());

        return new Password(password);
    }

    public override string ToString() => Value;
    public static implicit operator string(Password password) => password.Value;
}