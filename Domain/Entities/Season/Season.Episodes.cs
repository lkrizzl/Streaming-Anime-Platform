using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season
{
    public Episode AddEpisode(EpisodeNumber episodeNumber, Description title, TimeSpan duration)
    {
        if (Episodes.Any(e => e.EpisodeNumber == episodeNumber))
            throw new ValidationException(SeasonErrors.EpisodeNumberAlreadyExists(episodeNumber, Id));

        var episode = new Episode(Id, episodeNumber, title, duration);
        Episodes.Add(episode);
        EpisodesCount++;
        UpdatedOnUtc = UtcNow;
        return episode;
    }

    public void IncrementEpisodeCount()
    {
        EpisodesCount++;
        UpdatedOnUtc = UtcNow;
    }
}
