namespace Domain.Entities;

public partial class Anime
{
    public void SetCoverImage(string? coverImageUrl)
    {
        CoverImageUrl = string.IsNullOrWhiteSpace(coverImageUrl) ? null : coverImageUrl.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void SetBannerImage(string? bannerImageUrl)
    {
        BannerImageUrl = string.IsNullOrWhiteSpace(bannerImageUrl) ? null : bannerImageUrl.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void SetTrailerUrl(string? trailerUrl)
    {
        TrailerUrl = string.IsNullOrWhiteSpace(trailerUrl) ? null : trailerUrl.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void SetAgeRating(string? ageRating)
    {
        AgeRating = string.IsNullOrWhiteSpace(ageRating) ? null : ageRating.Trim();
        UpdatedOnUtc = UtcNow;
    }
}
