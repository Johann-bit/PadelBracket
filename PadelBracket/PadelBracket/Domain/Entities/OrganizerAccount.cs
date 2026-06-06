using System.Security.Cryptography;

namespace PadelBracket.Domain.Entities;

public class OrganizerAccount
{
    public Guid OrganizerId { get; private set; }
    public string Email { get; private set; }
    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }

    private OrganizerAccount()
    {
        Email = string.Empty;
        PasswordHash = Array.Empty<byte>();
        PasswordSalt = Array.Empty<byte>();
    }

    private OrganizerAccount(
        Guid organizerId,
        string email,
        byte[] passwordHash,
        byte[] passwordSalt)
    {
        OrganizerId = organizerId;
        Email = email.Trim().ToLower();
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public static OrganizerAccount Create(Guid organizerId, string email, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = HashPassword(password, salt);

        return new OrganizerAccount(
            organizerId,
            email,
            hash,
            salt);
    }

    public void ChangeEmail(string email)
    {
        Email = email.Trim().ToLower();
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