using System.Security.Cryptography;

namespace PadelBracket.Domain.Entities;

public class PlayerAccount
{
    public Guid PlayerId { get; private set; }
    public string Email { get; private set; }
    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }
    public string? PasswordResetCode { get; private set; }
    public DateTime? PasswordResetRequestedAt { get; private set; }

    private PlayerAccount()
    {
        Email = string.Empty;
        PasswordHash = Array.Empty<byte>();
        PasswordSalt = Array.Empty<byte>();
    }

    private PlayerAccount(
        Guid playerId,
        string email,
        byte[] passwordHash,
        byte[] passwordSalt)
    {
        PlayerId = playerId;
        Email = email.Trim().ToLower();
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public static PlayerAccount Create(Guid playerId, string email, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = HashPassword(password, salt);

        return new PlayerAccount(
            playerId,
            email,
            hash,
            salt);
    }

    public void ChangeEmail(string email)
    {
        Email = email.Trim().ToLower();
    }

    public void ChangePassword(string password)
    {
        PasswordSalt = RandomNumberGenerator.GetBytes(16);
        PasswordHash = HashPassword(password, PasswordSalt);
    }

    public string CreatePasswordResetCode()
    {
        PasswordResetCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        PasswordResetRequestedAt = DateTime.UtcNow;

        return PasswordResetCode;
    }

    public bool HasValidPasswordResetCode(string resetCode)
    {
        if (string.IsNullOrWhiteSpace(PasswordResetCode) || !PasswordResetRequestedAt.HasValue)
            return false;

        if (DateTime.UtcNow - PasswordResetRequestedAt.Value > TimeSpan.FromMinutes(15))
            return false;

        return string.Equals(PasswordResetCode, resetCode.Trim(), StringComparison.Ordinal);
    }

    public void ClearPasswordResetCode()
    {
        PasswordResetCode = null;
        PasswordResetRequestedAt = null;
    }

    public bool VerifyPassword(string password)
    {
        byte[] hash = HashPassword(password, PasswordSalt);

        return CryptographicOperations.FixedTimeEquals(hash, PasswordHash);
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);
    }
}