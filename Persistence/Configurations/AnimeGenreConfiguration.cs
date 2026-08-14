using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AnimeGenreConfiguration : IEntityTypeConfiguration<AnimeGenre>
{
    public void Configure(EntityTypeBuilder<AnimeGenre> builder)
    {
        builder.ToTable("AnimeGenres");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.AnimeId, x.GenreId })
            .IsUnique();
    }
}