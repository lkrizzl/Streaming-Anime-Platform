using Domain.Exceptions;

namespace Domain.Errors;

public static class NotesErrors
{
    public static Error NotesTooLong(int maxLength) =>
        new("Notes.TooLong", $"Notes must be at most {maxLength} characters.");
}