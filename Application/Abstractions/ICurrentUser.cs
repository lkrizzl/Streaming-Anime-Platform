namespace Application.Abstractions;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    public Guid? UserId { get; }

    public string? Name { get; }

    public string? Email { get; }
}
