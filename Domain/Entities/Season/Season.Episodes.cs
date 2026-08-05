using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Season
{
    public Episode AddEpisode(EpisodeNumber episodeNumber, Title title, TimeSpan duration)
    {
        //if (Episodes.Any(e => e.EpisodeNumber == episodeNumber))
            //throw new ValidationException($"Episode number {episodeNumber} already exists in this season.");

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
