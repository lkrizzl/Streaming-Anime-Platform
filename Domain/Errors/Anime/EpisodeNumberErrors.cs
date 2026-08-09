using Domain.Exceptions;

namespace Domain.Errors;

public static class EpisodeNumberErrors
{
    public static Error EpisodeNumberMustBePositive() =>
        new Error("EpisodeNumber", "Episode number must be greater than 0.");
}
