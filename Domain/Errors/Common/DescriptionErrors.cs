using Domain.Exceptions;

namespace Domain.Errors;

public static class DescriptionErrors
{
    public static Error DescriptionIsEmpty() =>
        new Error("Description", "Description cannot be empty.");

    public static Error DescriptionTooLong(int maxLength) =>
        new Error("Description", maxLength.ToString());
}
