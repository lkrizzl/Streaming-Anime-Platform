using Domain.Associations;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
    public DbSet<Anime> Anime => Set<Anime>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Studio> Studios => Set<Studio>();
    public DbSet<AnimeGenre> AnimeGenres => Set<AnimeGenre>();
    public DbSet<AnimeStudio> AnimeStudios => Set<AnimeStudio>();
    public DbSet<UserAnime> UserAnimes => Set<UserAnime>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUserIdentity(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureAnime(modelBuilder);
        ConfigureSeason(modelBuilder);
        ConfigureEpisode(modelBuilder);
        ConfigureGenre(modelBuilder);
        ConfigureStudio(modelBuilder);
        ConfigureAnimeGenre(modelBuilder);
        ConfigureAnimeStudio(modelBuilder);
        ConfigureUserAnime(modelBuilder);
    }

    private static void ConfigureUserIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserIdentity>(e =>
        {
            e.ToTable("UserIdentities");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.UserId)
                .IsRequired();

            e.HasIndex(x => x.UserId)
                .IsUnique();

            e.ComplexProperty(
                x => x.Username,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Username")
                        .HasMaxLength(Username.MaxLength)
                        .IsRequired();
                });

            e.ComplexProperty(
                x => x.Email,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Email")
                        .HasMaxLength(Email.MaxLength)
                        .IsRequired();
                });

            e.Property(x => x.PasswordHash)
                .HasMaxLength(512)
                .IsRequired();

            e.Property(x => x.SecurityStamp)
                .HasMaxLength(100)
                .IsRequired();
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.IdentityId)
                .IsRequired();

            e.HasIndex(x => x.IdentityId)
                .IsUnique();

            e.ComplexProperty(
                x => x.Username,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Username")
                        .HasMaxLength(Username.MaxLength)
                        .IsRequired();
                });

            e.ComplexProperty(
                x => x.Email,
                builder =>
                {
                    builder.Property(x => x.Value)
                        .HasColumnName("Email")
                        .HasMaxLength(Email.MaxLength)
                        .IsRequired();
                });

            e.Property(x => x.AvatarUrl)
                .HasMaxLength(500);

            e.Property(x => x.Bio)
                .HasMaxLength(1000);

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.IsBanned)
                .IsRequired();

            e.Property(x => x.Role)
                .HasMaxLength(20)
                .IsRequired();

            e.HasMany(u => u.UserAnimes)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<UserIdentity>()
                .WithOne()
                .HasForeignKey<UserIdentity>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureAnime(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>(e =>
        {
            e.ToTable("Anime");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            e.Property(x => x.OriginalTitle)
                .HasMaxLength(500)
                .IsRequired();

            e.Property(x => x.EnglishTitle)
                .HasMaxLength(500);

            e.Property(x => x.Description)
                .HasMaxLength(5000)
                .IsRequired();

            e.Property(x => x.ReleaseYear)
                .IsRequired();

            e.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            e.Property(x => x.CoverImageUrl)
                .HasMaxLength(2048);

            e.Property(x => x.BannerImageUrl)
                .HasMaxLength(2048);

            e.Property(x => x.TrailerUrl)
                .HasMaxLength(2048);

            e.Property(x => x.AverageRating);

            e.Property(x => x.RatingCount);

            e.Property(x => x.EpisodesCount);

            e.Property(x => x.CurrentEpisode);

            e.Property(x => x.AgeRating)
                .HasMaxLength(20);

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasMany(a => a.AnimeGenres)
                .WithOne(ag => ag.Anime)
                .HasForeignKey(ag => ag.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(a => a.AnimeStudios)
                .WithOne(ast => ast.Anime)
                .HasForeignKey(ast => ast.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(a => a.Seasons)
                .WithOne(s => s.Anime)
                .HasForeignKey(s => s.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(a => a.UserAnimes)
                .WithOne(ua => ua.Anime)
                .HasForeignKey(ua => ua.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Ignore(x => x.Genres);
            e.Ignore(x => x.Studios);

            e.HasIndex(x => x.Title);
        });
    }

    private static void ConfigureSeason(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Season>(e =>
        {
            e.ToTable("Seasons");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.SeasonNumber)
                .IsRequired();

            e.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();

            e.Property(x => x.Description)
                .HasMaxLength(2000)
                .IsRequired();

            e.Property(x => x.EpisodesCount);

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasMany(s => s.Episodes)
                .WithOne(ep => ep.Season)
                .HasForeignKey(ep => ep.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureEpisode(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Episode>(e =>
        {
            e.ToTable("Episodes");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.EpisodeNumber)
                .IsRequired();

            e.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();

            e.Property(x => x.Description)
                .HasMaxLength(2000);

            e.Property(x => x.Duration)
                .IsRequired();

            e.Property(x => x.VideoUrl)
                .HasMaxLength(1000);

            e.Property(x => x.ThumbnailUrl)
                .HasMaxLength(500);

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.IsPublished)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();
        });
    }

    private static void ConfigureGenre(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(e =>
        {
            e.ToTable("Genres");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.Description)
                .HasMaxLength(500);

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    private static void ConfigureStudio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Studio>(e =>
        {
            e.ToTable("Studios");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.Description)
                .HasMaxLength(2000);

            e.Property(x => x.LogoUrl)
                .HasMaxLength(500);

            e.Property(x => x.WebsiteUrl)
                .HasMaxLength(500);

            e.Property(x => x.IsActive)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    private static void ConfigureAnimeGenre(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimeGenre>(e =>
        {
            e.ToTable("AnimeGenres");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.HasIndex(x => new { x.AnimeId, x.GenreId })
                .IsUnique();
        });
    }

    private static void ConfigureAnimeStudio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimeStudio>(e =>
        {
            e.ToTable("AnimeStudios");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.HasIndex(x => new { x.AnimeId, x.StudioId })
                .IsUnique();
        });
    }

    private static void ConfigureUserAnime(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAnime>(e =>
        {
            e.ToTable("UserAnimes");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedNever();

            e.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            e.Property(x => x.Notes)
                .HasMaxLength(2000);

            e.Property(x => x.IsFavorite)
                .IsRequired();

            e.Property(x => x.CreatedOnUtc)
                .IsRequired();

            e.Property(x => x.LastUpdatedOnUtc)
                .IsRequired();

            e.HasIndex(x => new { x.UserId, x.AnimeId })
                .IsUnique();
        });
    }
}
