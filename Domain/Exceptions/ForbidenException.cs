namespace Domain.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access denied. You do not have permission to access this resource.") { }
}
