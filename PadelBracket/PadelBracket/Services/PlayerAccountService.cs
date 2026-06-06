using System.Text.RegularExpressions;
using Microsoft.JSInterop;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Services;

public class PlayerAccountService
{
    private const string CurrentPlayerStorageKey = "arenapadel.currentPlayerId";

    private readonly PlayerService playerService;
    private readonly IPlayerAccountRepository accountRepository;
    private readonly IJSRuntime? jsRuntime;
    private Guid? currentPlayerId;

    public event Action? SessionChanged;

    public PlayerAccountService(
        PlayerService playerService,
        IPlayerAccountRepository accountRepository)
    {
        this.playerService = playerService;
        this.accountRepository = accountRepository;
    }

    public PlayerAccountService(
        PlayerService playerService,
        IPlayerAccountRepository accountRepository,
        IJSRuntime jsRuntime)
        : this(playerService, accountRepository)
    {
        this.jsRuntime = jsRuntime;
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

        accountRepository.Add(PlayerAccount.Create(
            player.Id,
            player.Email,
            password));

        currentPlayerId = player.Id;
        SessionChanged?.Invoke();

        return player;
    }

    public async Task<Player> RegisterAsync(
        string realName,
        string email,
        string password,
        string confirmPassword,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        Player player = Register(
            realName,
            email,
            password,
            confirmPassword,
            dominantHand,
            preferredSide,
            category);

        await SaveCurrentPlayerAsync(player.Id);

        return player;
    }

    public Player Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es obligatoria.");

        PlayerAccount? account = accountRepository.GetByEmail(email);

        if (account == null || !account.VerifyPassword(password))
            throw new InvalidOperationException("Email o contraseña incorrectos.");

        currentPlayerId = account.PlayerId;
        SessionChanged?.Invoke();

        Player player = playerService.GetById(account.PlayerId)
            ?? throw new InvalidOperationException("No se encontró el jugador asociado a la cuenta.");

        return player;
    }

    public async Task<Player> LoginAsync(string email, string password)
    {
        Player player = Login(email, password);

        await SaveCurrentPlayerAsync(player.Id);

        return player;
    }

    public async Task RestoreSessionAsync()
    {
        if (currentPlayerId.HasValue || jsRuntime == null)
            return;

        try
        {
            string? storedPlayerId = await jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                CurrentPlayerStorageKey);

            if (!Guid.TryParse(storedPlayerId, out Guid playerId))
                return;

            if (playerService.GetById(playerId) == null)
                return;

            currentPlayerId = playerId;
            SessionChanged?.Invoke();
        }
        catch
        {
        }
    }

    public void Logout()
    {
        currentPlayerId = null;
        SessionChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        Logout();

        if (jsRuntime == null)
            return;

        try
        {
            await jsRuntime.InvokeVoidAsync(
                "localStorage.removeItem",
                CurrentPlayerStorageKey);
        }
        catch
        {
        }
    }

    public void UpdateCurrentPlayerPersonalData(string realName, string email)
    {
        Player player = CurrentPlayer
            ?? throw new InvalidOperationException("No hay una sesión activa.");

        ValidateRealName(realName);
        ValidateEmail(email);

        playerService.UpdatePersonalData(player.Id, realName, email);

        PlayerAccount account = accountRepository.GetByPlayerId(player.Id)
            ?? throw new InvalidOperationException("No se encontró la cuenta del jugador.");

        account.ChangeEmail(email);
        accountRepository.SaveChanges();

        SessionChanged?.Invoke();
    }

    public void ChangeCurrentPlayerPassword(
        string currentPassword,
        string newPassword,
        string confirmNewPassword)
    {
        Player player = CurrentPlayer
            ?? throw new InvalidOperationException("No hay una sesión activa.");

        PlayerAccount account = accountRepository.GetByPlayerId(player.Id)
            ?? throw new InvalidOperationException("No se encontró la cuenta del jugador.");

        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("La contraseña actual es obligatoria.");

        if (!account.VerifyPassword(currentPassword))
            throw new InvalidOperationException("La contraseña actual es incorrecta.");

        ValidatePassword(newPassword, confirmNewPassword, player.Name, player.Email);

        account.ChangePassword(newPassword);
        accountRepository.SaveChanges();
    }

    public string RequestPasswordReset(string email)
    {
        ValidateEmail(email);

        PlayerAccount account = accountRepository.GetByEmail(email)
            ?? throw new InvalidOperationException("No existe una cuenta registrada con ese email.");

        string resetCode = account.CreatePasswordResetCode();
        accountRepository.SaveChanges();

        return resetCode;
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

        PlayerAccount account = accountRepository.GetByEmail(email)
            ?? throw new InvalidOperationException("No existe una cuenta registrada con ese email.");

        Player player = playerService.GetById(account.PlayerId)
            ?? throw new InvalidOperationException("No se encontró el jugador asociado a la cuenta.");

        if (!account.HasValidPasswordResetCode(resetCode))
            throw new InvalidOperationException("El código de recuperación no es válido o venció.");

        ValidatePassword(newPassword, confirmNewPassword, player.Name, player.Email);

        account.ChangePassword(newPassword);
        account.ClearPasswordResetCode();
        accountRepository.SaveChanges();
    }

    private async Task SaveCurrentPlayerAsync(Guid playerId)
    {
        if (jsRuntime == null)
            return;

        try
        {
            await jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                CurrentPlayerStorageKey,
                playerId.ToString());
        }
        catch
        {
        }
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
}