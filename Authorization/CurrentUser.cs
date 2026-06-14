using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Authorization;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var raw = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? Name =>
        Principal?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Email =>
        Principal?.FindFirst(ClaimTypes.Email)?.Value;

    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;
}
