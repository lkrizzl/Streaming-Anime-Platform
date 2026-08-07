using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Password
{
    public const int MinLength = 8;
    public const int MaxLength = 64;

    private static readonly Regex PasswordRegex =
        new(@"^(?=.*[A-Za-z])(?=.*\d).{8,64}$", RegexOptions.Compiled);

    public string Value { get; init; }

    private Password(string value) => Value = value;

    public static Password Create(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException(PasswordErrors.PasswordIsEmpty());

        if (password.Length < MinLength)
            throw new ValidationException(PasswordErrors.PasswordTooShort(MinLength));

        if (password.Length > MaxLength)
            throw new ValidationException(PasswordErrors.PasswordTooLong(MaxLength));

        if (!PasswordRegex.IsMatch(password))
            throw new ValidationException(PasswordErrors.PasswordHasInvalidFormat());

        return new Password(password);
    }

    public override string ToString() => Value;
    public static implicit operator string(Password password) => password.Value;
}