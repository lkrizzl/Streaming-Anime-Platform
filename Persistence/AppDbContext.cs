using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
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

            e.Ignore(x => x.UserAnimes);

            e.HasOne<UserIdentity>()
                .WithOne()
                .HasForeignKey<UserIdentity>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
