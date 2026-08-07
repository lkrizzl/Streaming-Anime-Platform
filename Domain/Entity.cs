using Domain.Abstractions;

namespace Domain;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
    public static ITimeProvider TimeProvider { get; set; } = new SystemTimeProvider();
    protected static DateTime UtcNow => TimeProvider.UtcNow;
}
