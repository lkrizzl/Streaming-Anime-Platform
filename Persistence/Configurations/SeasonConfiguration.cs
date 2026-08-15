using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.ToTable("Seasons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.ComplexProperty(
            x => x.SeasonNumber,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("SeasonNumber")
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

        builder.ComplexProperty(
            x => x.Description,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(Synopsis.MaxLength)
                    .IsRequired();
            });

        builder.Property(x => x.EpisodesCount);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.HasMany(s => s.Episodes)
            .WithOne(ep => ep.Season)
            .HasForeignKey(ep => ep.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}