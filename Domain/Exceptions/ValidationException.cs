namespace Domain.Exceptions;

public class ValidationException : Exception
{

    public static readonly string ValidationErrorMessage = "The validation errors were occured.";

    public ValidationException(List<Error> errors) : base(ValidationErrorMessage)
    {
        Errors = errors;
    }

    public ValidationException(Error error) : base(ValidationErrorMessage)
    {
        Errors = [error];
    }

    public ValidationException() : base(ValidationErrorMessage)
    {
        Errors = [];
    }

    public List<Error> Errors { get; private set; }

    public void AddError(Error error)
    {
        Errors.Add(error);
    }

    public bool HasErrors()
    {
        return Errors.Count > 0;
    }
}