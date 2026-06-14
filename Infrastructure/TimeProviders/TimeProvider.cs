using Domain.Abstractions;

namespace Infrastructure.TimeProviders;

public class TimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
