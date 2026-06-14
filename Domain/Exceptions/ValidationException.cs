namespace Domain.Exceptions;

public class EntityValidationException : Exception
{
    public static readonly string ValidationErrorMessage = "The validation errors were occured.";

    public EntityValidationException(List<Error> errors) : base(ValidationErrorMessage)
    {
        Errors = errors;
    }

    public EntityValidationException(Error error) : base(ValidationErrorMessage)
    {
        Errors = [error];
    }

    public EntityValidationException() : base(ValidationErrorMessage)
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
