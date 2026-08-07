using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Email
{
    public const int MaxLength = 254;

    private static readonly Regex EmailRegex =
        new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    public string Value { get; init; }

    private Email(string value) => Value = value;

    public static Email Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException(EmailErrors.EmailIsEmpty());

        string trimmed = email.Trim().ToLowerInvariant();

        if (trimmed.Length > MaxLength)
            throw new ValidationException(EmailErrors.EmailTooLong(MaxLength));

        if (!EmailRegex.IsMatch(trimmed))
            throw new ValidationException(EmailErrors.EmailHasInvalidFormat());

        return new Email(trimmed);
    }

    public override string ToString() => Value;
    public static implicit operator string(Email email) => email.Value;
}