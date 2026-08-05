using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ProgressPercent
{
    public const double MinValue = 0.0;
    public const double MaxValue = 100.0;

    public double Value { get; init; }

    private ProgressPercent(double value) => Value = value;

    public static ProgressPercent Create(double percent)
    {
        if (percent < MinValue || percent > MaxValue)
            throw new EntityValidationException(
                ProgressPercentErrors.ProgressPercentOutOfRange(MinValue, MaxValue));

        return new ProgressPercent(Math.Round(percent, 1));
    }

    public override string ToString() => $"{Value:F1}%";
    public static implicit operator double(ProgressPercent percent) => percent.Value;
}
