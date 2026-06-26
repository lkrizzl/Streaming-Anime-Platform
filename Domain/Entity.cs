namespace Domain;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }

    /// <summary>
    /// Time provider for entity timestamps. Can be replaced in tests with a fake.
    /// Defaults to <see cref="System.TimeProvider.System"/>.
    /// </summary>
    public static TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// Gets the current UTC date and time from <see cref="TimeProvider"/>.
    /// </summary>
    protected static DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;
}
