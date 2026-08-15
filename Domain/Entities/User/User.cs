using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User : Entity
{
    private User() : base(Guid.NewGuid()) { } 

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

    public Guid IdentityId { get; private init; }

    public Username Username { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public ImageUrl? AvatarUrl { get; private set; }
    public Bio? Bio { get; private set; }
    public DateTime CreatedOnUtc { get; private init; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public DateTime? LastLoginOnUtc { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsBanned { get; private set; } = false;
    public DateTime? BannedUntilUtc { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;
    private readonly List<UserAnime> _userAnimes = new();
    public IReadOnlyCollection<UserAnime> UserAnimes => _userAnimes.AsReadOnly();

    public void UpdateUsername(Username newUsername)
    {
        Username = newUsername;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl is not null ? ImageUrl.Create(avatarUrl) : null;
        UpdatedOnUtc = UtcNow;
    }

    public void UpdateBio(string? bio)
    {
        Bio = Bio.Create(bio);
        UpdatedOnUtc = UtcNow;
    }

    public UserAnime AddToWatchlist(Guid animeId, WatchStatus status)
    {
        var userAnime = new UserAnime(Id, animeId, status);
        _userAnimes.Add(userAnime);
        UpdatedOnUtc = UtcNow;
        return userAnime;
    }

    public UserAnime? RemoveFromWatchlist(Guid animeId)
    {
        var userAnime = UserAnimes.FirstOrDefault(ua => ua.AnimeId == animeId);
        if (userAnime is not null)
        {
            _userAnimes.Remove(userAnime);
            UpdatedOnUtc = UtcNow;
        }
        return userAnime;
    }

    public void RecordLogin()
    {
        LastLoginOnUtc = UpdatedOnUtc = UtcNow;
    }
}