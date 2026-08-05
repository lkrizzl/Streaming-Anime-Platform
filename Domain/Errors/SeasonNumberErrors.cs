using Domain.Exceptions;

namespace Domain.Errors;

public static class SeasonNumberErrors
{
    public static Error SeasonNumberMustBePositive() =>
        new Error("SeasonNumber", "Season number must be greater than 0.");
}
