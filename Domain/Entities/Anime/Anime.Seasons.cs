using Domain.ValueObjects;
using System.Linq;

namespace Domain.Entities;

public partial class Anime
{
    public Season AddSeason(SeasonNumber seasonNumber, Title title, string description)
    {
        //if (Seasons.Any(s => s.SeasonNumber == seasonNumber))
            //throw new ValidationException($"Season number {seasonNumber} already exists for this anime.");

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
