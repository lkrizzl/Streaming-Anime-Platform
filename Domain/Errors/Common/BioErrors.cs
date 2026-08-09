using Domain.Exceptions;

namespace Domain.Errors;

public static class BioErrors
{
    public static Error BioTooLong(int maxLength) => new Error(
        "BioTooLong",
        $"Bio cannot exceed {maxLength} characters."
    );

}
