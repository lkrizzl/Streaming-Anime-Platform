using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization;

public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddScoped<AppCookieEvents>();
        services.AddScoped<IClaimsPrincipalProvider, ClaimsPrincipalProvider>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
