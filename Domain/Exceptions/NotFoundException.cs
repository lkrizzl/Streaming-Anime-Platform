namespace Domain.Exceptions;

public class NotFoundException : Exception
{
    public Error? Error { get; }

    public NotFoundException(Error error) : base(error.Message)
    {
        Error = error;
    }

    public NotFoundException(string message) : base(message)
    {
        Error = null;
    }
}
