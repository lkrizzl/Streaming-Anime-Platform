using Domain.Entities;
using Domain.ValueObjects.UserObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserAnimeConfiguration : IEntityTypeConfiguration<UserAnime>
{
    public void Configure(EntityTypeBuilder<UserAnime> builder)
    {
        builder.ToTable("UserAnimes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.ComplexProperty(
            x => x.Notes,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Notes")
                    .HasMaxLength(Notes.MaxLength);
            });

        builder.ComplexProperty(
            x => x.LastWatchedEpisodeNumber,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("LastWatchedEpisodeNumber");
            });

        builder.ComplexProperty(
            x => x.ProgressPercentage,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("ProgressPercentage");
            });

        builder.ComplexProperty(
            x => x.UserRating,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("UserRating");
            });

        builder.Property(x => x.IsFavorite)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.Property(x => x.LastUpdatedOnUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.AnimeId })
            .IsUnique();
    }
}