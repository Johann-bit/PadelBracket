using System.Text.RegularExpressions;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Services;

public class OrganizerAccountService
{
    private readonly OrganizerService organizerService;
    private readonly IOrganizerAccountRepository accountRepository;
    private Guid? currentOrganizerId;

    public OrganizerAccountService(
        OrganizerService organizerService,
        IOrganizerAccountRepository accountRepository)
    {
        this.organizerService = organizerService;
        this.accountRepository = accountRepository;
    }

    public bool IsLoggedIn => currentOrganizerId.HasValue;

    public Organizer? CurrentOrganizer
    {
        get
        {
            if (!currentOrganizerId.HasValue)
                return null;

            return organizerService.GetById(currentOrganizerId.Value);
        }
    }

    public Organizer Register(
        string realName,
        string email,
        string password,
        string confirmPassword,
        string clubName,
        string city,
        string phone)
    {
        ValidateRealName(realName);
        ValidateEmail(email);
        ValidatePassword(password, confirmPassword, realName, email);

        if (EmailAlreadyExists(email))
            throw new ArgumentException("Ya existe una cuenta de organizador registrada con ese email.");

        Organizer organizer = organizerService.Add(
            realName,
            email,
            clubName,
            city,
            phone);

        accountRepository.Add(OrganizerAccount.Create(
            organizer.Id,
            organizer.Email,
            password));

        currentOrganizerId = organizer.Id;

        return organizer;
    }

    public Organizer Login(string email, string password)
    {
        ValidateEmail(email);

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es obligatoria.");

        OrganizerAccount? account = accountRepository.GetByEmail(email);

        if (account == null || !account.VerifyPassword(password))
            throw new InvalidOperationException("Email o contraseña incorrectos.");

        currentOrganizerId = account.OrganizerId;

        Organizer organizer = organizerService.GetById(account.OrganizerId)
            ?? throw new InvalidOperationException("No se encontró el organizador asociado a la cuenta.");

        return organizer;
    }

    public void Logout()
    {
        currentOrganizerId = null;
    }

    public void UpdateCurrentOrganizerProfile(
        string realName,
        string email,
        string clubName,
        string city,
        string phone)
    {
        Organizer organizer = CurrentOrganizer
            ?? throw new InvalidOperationException("No hay una sesión activa de organizador.");

        ValidateRealName(realName);
        ValidateEmail(email);

        organizerService.UpdateProfile(
            organizer.Id,
            realName,
            email,
            clubName,
            city,
            phone);

        OrganizerAccount account = accountRepository.GetByOrganizerId(organizer.Id)
            ?? throw new InvalidOperationException("No se encontró la cuenta del organizador.");

        account.ChangeEmail(email);
        accountRepository.SaveChanges();
    }

    private bool EmailAlreadyExists(string email)
    {
        return organizerService.GetAll().Any(organizer =>
            string.Equals(organizer.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
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