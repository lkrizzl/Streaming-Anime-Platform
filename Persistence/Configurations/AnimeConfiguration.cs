using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AnimeConfiguration : IEntityTypeConfiguration<Anime>
{
    public void Configure(EntityTypeBuilder<Anime> builder)
    {
        builder.ToTable("Anime");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.ComplexProperty(
            x => x.Title,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Title")
                    .HasMaxLength(500)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.OriginalTitle,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("OriginalTitle")
                    .HasMaxLength(500)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.EnglishTitle,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("EnglishTitle")
                    .HasMaxLength(500);
            });

        builder.ComplexProperty(
            x => x.Description,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(Synopsis.MaxLength)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.ReleaseYear,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("ReleaseYear")
                    .IsRequired();
            });

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.ComplexProperty(
            x => x.CoverImageUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("CoverImageUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.ComplexProperty(
            x => x.BannerImageUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("BannerImageUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.ComplexProperty(
            x => x.TrailerUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("TrailerUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.ComplexProperty(
            x => x.AverageRating,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("AverageRating")
                    .IsRequired();
            });

        builder.Property(x => x.RatingCount);
        builder.Property(x => x.EpisodesCount);
        builder.Property(x => x.CurrentEpisode);

        builder.ComplexProperty(
            x => x.AgeRating,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("AgeRating")
                    .HasMaxLength(20);
            });

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.HasMany(a => a.AnimeGenres)
            .WithOne(ag => ag.Anime)
            .HasForeignKey(ag => ag.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.AnimeStudios)
            .WithOne(ast => ast.Anime)
            .HasForeignKey(ast => ast.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Seasons)
            .WithOne(s => s.Anime)
            .HasForeignKey(s => s.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.UserAnimes)
            .WithOne(ua => ua.Anime)
            .HasForeignKey(ua => ua.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.Genres);
        builder.Ignore(x => x.Studios);
    }
}