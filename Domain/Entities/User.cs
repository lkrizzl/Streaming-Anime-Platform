//================ Сутність користувача ==================
using Domain.Associations;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User : Entity
{
    private User() : base(Guid.NewGuid()) { } // Для EF Core

    public User(
        Guid identityId,
        Username username,
        Email email)
        : base(Guid.NewGuid())
    {
        IdentityId = identityId;
        Username = username;
        Email = email;

        CreatedOnUtc = UtcNow;
        IsActive = true;
    }

    // ====================== Основні дані ======================
    public Guid IdentityId { get; private init; }

    public Username Username { get; private set; }
    public Email Email { get; private set; }

    // ====================== Профіль ======================
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }

    // ====================== Аудит ======================
    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public DateTime? LastLoginOnUtc { get; private set; }

    // ====================== Статус ======================
    public bool IsActive { get; private set; } = true;
    public bool IsBanned { get; private set; } = false;
    public DateTime? BannedUntilUtc { get; private set; }

    // ====================== Відносини ======================
    public ICollection<UserAnime> UserAnimes { get; private set; } = new List<UserAnime>();

    // ====================== Бізнес методи ======================

    public void UpdateUsername(Username newUsername)
    {
        Username = newUsername ?? throw new EntityValidationException(UsernameErrors.UsernameIsEmpty());
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new EntityValidationException(EmailErrors.EmailIsEmpty());
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateBio(string? bio)
    {
        Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        UpdatedOnUtc = UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginOnUtc = UpdatedOnUtc = UtcNow;
    }
}