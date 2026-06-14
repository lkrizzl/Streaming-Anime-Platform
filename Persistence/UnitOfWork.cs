using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
