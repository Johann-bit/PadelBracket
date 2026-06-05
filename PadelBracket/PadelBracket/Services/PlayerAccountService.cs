using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
        return Register(
            realName,
            email,
            password,
            password,
            dominantHand,
            preferredSide,
            category);
    }

    public Player Register(
        string realName,
        string email,
        string password,
        string confirmPassword,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        ValidateRealName(realName);
        ValidateEmail(email);
        ValidatePassword(password, confirmPassword, realName, email);

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

    public void UpdateCurrentPlayerPersonalData(string realName, string email)
    {
        Player player = CurrentPlayer
            ?? throw new InvalidOperationException("No hay una sesión activa.");

        ValidateRealName(realName);
        ValidateEmail(email);

        playerService.UpdatePersonalData(player.Id, realName, email);

        if (accountsByPlayerId.TryGetValue(player.Id, out PlayerAccount? account))
            account.ChangeEmail(email);
    }

    public void ChangeCurrentPlayerPassword(
        string currentPassword,
        string newPassword,
        string confirmNewPassword)
    {
        Player player = CurrentPlayer
            ?? throw new InvalidOperationException("No hay una sesión activa.");

        PlayerAccount account = accountsByPlayerId.GetValueOrDefault(player.Id)
            ?? throw new InvalidOperationException("No se encontró la cuenta del jugador.");

        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("La contraseña actual es obligatoria.");

        if (!account.VerifyPassword(currentPassword))
            throw new InvalidOperationException("La contraseña actual es incorrecta.");

        ValidatePassword(newPassword, confirmNewPassword, player.Name, player.Email);

        account.ChangePassword(newPassword);
    }

    public string RequestPasswordReset(string email)
    {
        ValidateEmail(email);

        PlayerAccount account = accountsByPlayerId.Values.FirstOrDefault(account =>
            string.Equals(account.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("No existe una cuenta registrada con ese email.");

        return account.CreatePasswordResetCode();
    }

    public void ResetPassword(
        string email,
        string resetCode,
        string newPassword,
        string confirmNewPassword)
    {
        ValidateEmail(email);

        if (string.IsNullOrWhiteSpace(resetCode))
            throw new ArgumentException("El código de recuperación es obligatorio.");

        PlayerAccount account = accountsByPlayerId.Values.FirstOrDefault(account =>
            string.Equals(account.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("No existe una cuenta registrada con ese email.");

        Player player = playerService.GetById(account.PlayerId)
            ?? throw new InvalidOperationException("No se encontró el jugador asociado a la cuenta.");

        if (!account.HasValidPasswordResetCode(resetCode))
            throw new InvalidOperationException("El código de recuperación no es válido o venció.");

        ValidatePassword(newPassword, confirmNewPassword, player.Name, player.Email);

        account.ChangePassword(newPassword);
        account.ClearPasswordResetCode();
    }

    private bool EmailAlreadyExists(string email)
    {
        return playerService.GetAll().Any(player =>
            string.Equals(player.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidateRealName(string realName)
    {
        if (string.IsNullOrWhiteSpace(realName))
            throw new ArgumentException("El nombre real es obligatorio.");

        string trimmedName = realName.Trim();

        if (trimmedName.Length < 3)
            throw new ArgumentException("El nombre real debe tener al menos 3 caracteres.");

        if (trimmedName.Length > 80)
            throw new ArgumentException("El nombre real no puede superar los 80 caracteres.");

        if (!trimmedName.Contains(' '))
            throw new ArgumentException("Ingresá nombre y apellido.");

        if (trimmedName.Any(char.IsDigit))
            throw new ArgumentException("El nombre real no puede contener números.");

        if (!Regex.IsMatch(trimmedName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ' -]+$"))
            throw new ArgumentException("El nombre real solo puede contener letras, espacios, apóstrofe o guion.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        string trimmedEmail = email.Trim();

        if (trimmedEmail.Length > 120)
            throw new ArgumentException("El email no puede superar los 120 caracteres.");

        if (!Regex.IsMatch(trimmedEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("El email no tiene un formato válido.");
    }

    private static void ValidatePassword(
        string password,
        string confirmPassword,
        string realName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es obligatoria.");

        if (password != confirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");

        if (password.Length < 8)
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");

        if (password.Length > 100)
            throw new ArgumentException("La contraseña no puede superar los 100 caracteres.");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("La contraseña debe incluir al menos una mayúscula.");

        if (!password.Any(char.IsLower))
            throw new ArgumentException("La contraseña debe incluir al menos una minúscula.");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("La contraseña debe incluir al menos un número.");

        if (!password.Any(character => !char.IsLetterOrDigit(character)))
            throw new ArgumentException("La contraseña debe incluir al menos un carácter especial.");

        string normalizedPassword = password.ToLowerInvariant();
        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(normalizedEmail) &&
            normalizedPassword.Contains(normalizedEmail))
        {
            throw new ArgumentException("La contraseña no puede contener tu email.");
        }

        foreach (string namePart in realName.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (namePart.Length >= 3 && normalizedPassword.Contains(namePart))
                throw new ArgumentException("La contraseña no puede contener partes de tu nombre.");
        }
    }

    private class PlayerAccount
    {
        public Guid PlayerId { get; }
        public string Email { get; private set; }
        private byte[] PasswordHash { get; set; }
        private byte[] PasswordSalt { get; set; }
        private string? PasswordResetCode { get; set; }
        private DateTime? PasswordResetRequestedAt { get; set; }

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
}