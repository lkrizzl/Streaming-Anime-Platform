using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class UserIdentityService(AppDbContext dbContext) : IUserIdentityService
{
    public async Task AddAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        await dbContext.UserIdentities.AddAsync(userIdentity, cancellationToken);
    }

    public async Task<bool> ExistsByEmailOrUsernameAsync(Email email, Username username, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserIdentities.AnyAsync(
            ui => ui.Email.Value == email.Value || ui.Username.Value == username.Value,
            cancellationToken);
    }

    public async Task<UserIdentity?> FindByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = emailOrUsername.Trim().ToLowerInvariant();
        var normalizedUsername = emailOrUsername.Trim();

        return await dbContext.UserIdentities
            .FirstOrDefaultAsync(
                ui => ui.Email.Value == normalizedEmail || ui.Username.Value == normalizedUsername,
                cancellationToken);
    }

    public async Task<UserIdentity?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserIdentities.FirstOrDefaultAsync(ui => ui.UserId == userId, cancellationToken);
    }

    public async Task RemoveAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        await dbContext.UserIdentities.Where(ui => ui.UserId == userIdentity.UserId).ExecuteDeleteAsync(cancellationToken);
    }
}
