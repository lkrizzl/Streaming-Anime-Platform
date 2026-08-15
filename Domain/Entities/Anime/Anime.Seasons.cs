using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using System.Linq;

namespace Domain.Entities;

public partial class Anime
{
    public Season AddSeason(SeasonNumber seasonNumber, Description title, string description)
    {
        if (Seasons.Any(s => s.SeasonNumber == seasonNumber))
            throw new ValidationException(SeasonErrors.DuplicateSeasonNumber(seasonNumber));

        var season = new Season(Id, seasonNumber, title, Synopsis.Create(description));
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
