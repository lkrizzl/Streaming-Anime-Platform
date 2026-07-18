using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class UserIdentity : Entity
{
    private UserIdentity() : base(Guid.NewGuid()) { }

    public UserIdentity(Guid id, Guid userId, Username username, Email email, Password password, IPasswordHasher passwordHasher) : base(id)
    {
        UserId = userId;
        Username = username;
        Email = email;
        PasswordHash = passwordHasher.HashPassword(password.Value);
        SecurityStamp = Guid.NewGuid().ToString();
        CreatedOnUtc = UtcNow;
    }

    public Guid UserId { get; init; }

    public Username Username { get; init; }

    public Email Email { get; init; }

    public string PasswordHash { get; private set; }

    public string SecurityStamp { get; private set; }

    public DateTime CreatedOnUtc { get; private init; }

    public void UpdatePassword(Password password, IPasswordHasher passwordHasher)
    {
        PasswordHash = passwordHasher.HashPassword(password.Value);
    }

    public void UpdateSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid().ToString();
    }
}
