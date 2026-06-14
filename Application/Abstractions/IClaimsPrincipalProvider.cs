using System.Security.Claims;

namespace Application.Abstractions;

public interface IClaimsPrincipalProvider
{
    ClaimsPrincipal Create(Guid userId, string username, string email, string sstamp);
}
