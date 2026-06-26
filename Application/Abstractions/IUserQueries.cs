using Domain.Entities;

namespace Application.Abstractions;

public record UserResponse(
    Guid Id,
    string Name,
    string Email
); 


public interface IUserQueries
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
