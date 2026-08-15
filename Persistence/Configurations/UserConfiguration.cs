using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.IdentityId)
            .IsRequired();

        builder.HasIndex(x => x.IdentityId)
            .IsUnique();

        builder.ComplexProperty(
            x => x.Username,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Username")
                    .HasMaxLength(Username.MaxLength)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.Email,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(Email.MaxLength)
                    .IsRequired();
            });

        builder.ComplexProperty(
            x => x.AvatarUrl,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("AvatarUrl")
                    .HasMaxLength(ImageUrl.MaxLength);
            });

        builder.ComplexProperty(
            x => x.Bio,
            cb =>
            {
                cb.Property(x => x.Value)
                    .HasColumnName("Bio")
                    .HasMaxLength(Bio.MaxLength);
            });

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsBanned)
            .IsRequired();

        builder.HasMany(u => u.UserAnimes)
            .WithOne(ua => ua.User)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserIdentity>()
            .WithOne()
            .HasForeignKey<UserIdentity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}