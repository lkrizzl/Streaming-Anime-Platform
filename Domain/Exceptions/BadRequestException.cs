namespace Domain.Exceptions;

public class BadRequestException : Exception
{
    public Error? Error { get; }

    public BadRequestException(Error error) : base(error.Message)
    {
        Error = error;
    }

    public BadRequestException(string message) : base(message)
    {
        Error = null;
    }
}
