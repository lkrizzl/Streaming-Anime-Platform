using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AnimeStudioConfiguration : IEntityTypeConfiguration<AnimeStudio>
{
    public void Configure(EntityTypeBuilder<AnimeStudio> builder)
    {
        builder.ToTable("AnimeStudios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.AnimeId, x.StudioId })
            .IsUnique();
    }
}