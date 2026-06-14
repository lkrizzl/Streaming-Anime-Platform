namespace Domain.Abstractions;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
