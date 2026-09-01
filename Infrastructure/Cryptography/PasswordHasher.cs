using Domain.Abstractions;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Cryptography;
public class PasswordHasher : IPasswordHasher
{
    private const string Argon2Prefix = "$argon2id$v1$";

    private const int Argon2SaltSize = 16;
    private const int Argon2HashSize = 32;
    private const int Argon2MemoryKb = 19 * 1024;
    private const int Argon2Iterations = 2;
    private const int Argon2Parallelism = 1;
    private const int Pbkdf2HashSize = 32;
    private const int Pbkdf2Iterations = 100_000;
    private static readonly HashAlgorithmName Pbkdf2Algorithm = HashAlgorithmName.SHA512;

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(Argon2SaltSize);
        byte[] hash = HashWithArgon2id(password, salt);

        return $"{Argon2Prefix}{Convert.ToBase64String(hash)}${Convert.ToBase64String(salt)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return passwordHash.StartsWith(Argon2Prefix, StringComparison.Ordinal)
            ? VerifyArgon2id(password, passwordHash)
            : VerifyLegacyPbkdf2(password, passwordHash);
    }

    public bool NeedsRehash(string passwordHash) =>
        !passwordHash.StartsWith(Argon2Prefix, StringComparison.Ordinal);

    private static bool VerifyArgon2id(string password, string passwordHash)
    {
        string[] parts = passwordHash[Argon2Prefix.Length..].Split('$');
        if (parts.Length != 2)
        {
            return false;
        }

        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] computedHash = HashWithArgon2id(password, salt);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }

    private static byte[] HashWithArgon2id(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Argon2Parallelism,
            Iterations = Argon2Iterations,
            MemorySize = Argon2MemoryKb,
        };

        return argon2.GetBytes(Argon2HashSize);
    }

    private static bool VerifyLegacyPbkdf2(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        if (parts.Length != 2)
        {
            return false;
        }

        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Pbkdf2Iterations, Pbkdf2Algorithm, Pbkdf2HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}