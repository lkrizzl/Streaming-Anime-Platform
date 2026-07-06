using Application.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Authorization;

public sealed class AppCookieEvents(IUserIdentityService userIdentityService) : CookieAuthenticationEvents
{
    private static readonly string SecurityStampKey = AuthConstants.SecurityStampClaimType;
    private static readonly string LastValidatedTimeKey = AuthConstants.LastValidatedTimeKey;
    private static readonly TimeSpan ValidationInterval = TimeSpan.FromMinutes(10);

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private static bool TryGetLastValidatedUtc(AuthenticationProperties props, out DateTimeOffset utc)
    {
        utc = default;

        if (props.Items.TryGetValue(LastValidatedTimeKey, out var s) &&
            long.TryParse(s, out var unix))
        {
            utc = DateTimeOffset.FromUnixTimeSeconds(unix);
            return true;
        }

        if (props.IssuedUtc is { } issuedUtc)
        {
            utc = issuedUtc;
            return true;
        }

        return false;
    }

    private static void SetLastValidatedUtc(AuthenticationProperties props, DateTimeOffset utc)
        => props.Items[LastValidatedTimeKey] = utc.ToUnixTimeSeconds().ToString();

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal;

        if (principal is null)
        {
            await RejectAsync(context);
            return;
        }

        var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var stampInCookie = principal.FindFirst(SecurityStampKey)?.Value;

        if (!Guid.TryParse(userIdString, out var userId) || string.IsNullOrEmpty(stampInCookie))
        {
            await RejectAsync(context);
            return;
        }

        var now = DateTimeOffset.UtcNow;

        if (TryGetLastValidatedUtc(context.Properties, out var lastValidatedUtc)
            && now - lastValidatedUtc < ValidationInterval)
        {
            return;
        }

        var identity = await userIdentityService.FindByUserIdAsync(userId, context.HttpContext.RequestAborted);

        var currentStamp = identity?.SecurityStamp;

        if (currentStamp is null || !string.Equals(currentStamp, stampInCookie, StringComparison.Ordinal))
        {
            await RejectAsync(context);
            return;
        }

        SetLastValidatedUtc(context.Properties, now);
        context.ShouldRenew = true;
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}
