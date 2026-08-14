using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("Episodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.ComplexProperty(
            x => x.EpisodeNumber,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("EpisodeNumber")
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.Title,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Title")
                    .HasMaxLength(500)
                    .IsRequired();
            });

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Duration)
            .IsRequired();

        builder.ComplexProperty(
            x => x.VideoUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("VideoUrl")
                    .HasMaxLength(Domain.ValueObjects.AnimeObjects.VideoUrl.MaxLength);
            });

        builder.ComplexProperty(
            x => x.ThumbnailUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("ThumbnailUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();
    }
}