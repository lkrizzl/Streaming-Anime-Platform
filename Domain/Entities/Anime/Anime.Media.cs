using Domain.ValueObjects;

namespace Domain.Entities;

public partial class Anime
{
    public void SetCoverImage(string? coverImageUrl)
    {
        CoverImageUrl = coverImageUrl is not null ? ImageUrl.Create(coverImageUrl) : null;
        UpdatedOnUtc = UtcNow;
    }

    public void SetBannerImage(string? bannerImageUrl)
    {
        BannerImageUrl = bannerImageUrl is not null ? ImageUrl.Create(bannerImageUrl) : null;
        UpdatedOnUtc = UtcNow;
    }

    public void SetTrailerUrl(string? trailerUrl)
    {
        TrailerUrl = trailerUrl is not null ? ImageUrl.Create(trailerUrl) : null;
        UpdatedOnUtc = UtcNow;
    }

    public void SetAgeRating(string? ageRating)
    {
        AgeRating = string.IsNullOrWhiteSpace(ageRating) ? null : ageRating.Trim();
        UpdatedOnUtc = UtcNow;
    }
}
