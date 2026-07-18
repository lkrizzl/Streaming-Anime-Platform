using Domain.Exceptions;
using System.Linq;

namespace Domain.Entities;

public partial class Anime
{
    public Season AddSeason(int seasonNumber, string title, string description)
    {
        if (seasonNumber < 1)
            throw new ValidationException("Season number must be greater than 0.");

        if (Seasons.Any(s => s.SeasonNumber == seasonNumber))
            throw new ValidationException($"Season number {seasonNumber} already exists for this anime.");

        var season = new Season(Id, seasonNumber, title, description);
        Seasons.Add(season);
        UpdatedOnUtc = UtcNow;
        return season;
    }

    public void RemoveSeason(Guid seasonId)
    {
        var season = Seasons.FirstOrDefault(s => s.Id == seasonId);
        if (season is not null)
        {
            Seasons.Remove(season);
            UpdatedOnUtc = UtcNow;
        }
    }
}
