using Application.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Authorization;

public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
{
    public ClaimsPrincipal Create(Guid userId, string username, string email, string sstamp, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
            new(AuthConstants.SecurityStampClaimType, sstamp),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
