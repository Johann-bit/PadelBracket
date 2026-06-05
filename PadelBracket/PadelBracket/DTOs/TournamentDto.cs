namespace PadelBracket.Domain.DTOs;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public int GroupCount { get; set; }

    public List<TournamentCategoryDto> Categories { get; set; } = new();
    public List<TournamentRegistrationDto> Registrations { get; set; } = new();

    public int CategoryCount => Categories.Count;
    public int RegistrationCount => Registrations.Count;

    public int ActiveRegistrationCount => Registrations.Count(registration =>
        registration.Status == Domain.Enums.RegistrationStatus.Pending ||
        registration.Status == Domain.Enums.RegistrationStatus.Confirmed);

    public int ConfirmedRegistrationCount => Registrations.Count(registration =>
        registration.Status == Domain.Enums.RegistrationStatus.Confirmed);

    public int PendingRegistrationCount => Registrations.Count(registration =>
        registration.Status == Domain.Enums.RegistrationStatus.Pending);

    public bool HasCategories => Categories.Any();
    public bool HasRegistrations => Registrations.Any();

    public string CreatedAtLabel => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    public string StartDateLabel => StartDate.ToString("dd/MM/yyyy");
}