using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UserIdentityConfiguration : IEntityTypeConfiguration<UserIdentity>
{
    public void Configure(EntityTypeBuilder<UserIdentity> builder)
    {
        builder.ToTable("UserIdentities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
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

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.SecurityStamp)
            .HasMaxLength(100)
            .IsRequired();
    }
}