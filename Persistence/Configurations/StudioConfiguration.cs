using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class StudioConfiguration : IEntityTypeConfiguration<Studio>
{
    public void Configure(EntityTypeBuilder<Studio> builder)
    {
        builder.ToTable("Studios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.ComplexProperty(
            x => x.Name,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(StudioName.MaxLength)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.Description,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(Synopsis.MaxLength);
            });

        builder.ComplexProperty(
            x => x.LogoUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("LogoUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();
    }
}