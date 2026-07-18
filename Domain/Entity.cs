namespace Domain;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
    public static TimeProvider TimeProvider { get; set; } = TimeProvider.System;
    protected static DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;
}
