using System.Security.Cryptography;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Services;

public class PlayerAccountService
{
    private readonly PlayerService playerService;
    private readonly Dictionary<Guid, PlayerAccount> accountsByPlayerId = new();
    private Guid? currentPlayerId;

    public PlayerAccountService(PlayerService playerService)
    {
        this.playerService = playerService;
    }

    public bool IsLoggedIn => currentPlayerId.HasValue;

    public Player? CurrentPlayer
    {
        get
        {
            if (!currentPlayerId.HasValue)
                return null;

            return playerService.GetById(currentPlayerId.Value);
        }
    }

    public Player Register(
        string realName,
        string email,
        string password,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        ValidatePassword(password);

        if (EmailAlreadyExists(email))
            throw new ArgumentException("Ya existe una cuenta registrada con ese email.");

        Player player = playerService.Add(
            realName,
            email,
            dominantHand,
            preferredSide,
            category);

        accountsByPlayerId[player.Id] = PlayerAccount.Create(
            player.Id,
            player.Email,
            password);

        currentPlayerId = player.Id;

        return player;
    }

    public Player Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es obligatoria.");

        PlayerAccount? account = accountsByPlayerId.Values.FirstOrDefault(account =>
            string.Equals(account.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (account == null || !account.VerifyPassword(password))
            throw new InvalidOperationException("Email o contraseña incorrectos.");

        currentPlayerId = account.PlayerId;

        Player player = playerService.GetById(account.PlayerId)
            ?? throw new InvalidOperationException("No se encontró el jugador asociado a la cuenta.");

        return player;
    }

    public void Logout()
    {
        currentPlayerId = null;
    }

    private bool EmailAlreadyExists(string email)
    {
        return playerService.GetAll().Any(player =>
            string.Equals(player.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es obligatoria.");

        if (password.Length < 6)
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
    }

    private class PlayerAccount
    {
        public Guid PlayerId { get; }
        public string Email { get; }
        private byte[] PasswordHash { get; }
        private byte[] PasswordSalt { get; }

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
}