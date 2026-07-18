namespace Domain.Entities;

public partial class Anime
{
    public void UpdateRating(double newAverage, int newCount)
    {
        AverageRating = newAverage;
        RatingCount = newCount;
        UpdatedOnUtc = UtcNow;
    }

    public void ChangeStatus(AnimeStatus newStatus)
    {
        Status = newStatus;
        UpdatedOnUtc = UtcNow;
    }
}
