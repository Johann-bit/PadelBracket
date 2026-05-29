using PadelBracket.Domain.Entities;

namespace PadelBracket.Domain.DTOs;

public class TournamentCategoryDto
{
    public Guid Id { get; set; }
    public int Category { get; set; }
    public int MaxPairs { get; set; }
    public decimal RegistrationFee { get; set; }
    public int ActiveRegistrationsCount { get; set; }
    public int AvailableSlots => MaxPairs - ActiveRegistrationsCount;
    public bool HasAvailableSlots => AvailableSlots > 0;

    public string CategoryLabel => $"{Category}ta";
    public string RegistrationFeeLabel => $"${RegistrationFee}";
    public string SlotsLabel => $"{ActiveRegistrationsCount}/{MaxPairs}";

    public static TournamentCategoryDto FromEntity(
        TournamentCategory tournamentCategory,
        int activeRegistrationsCount = 0)
    {
        return new TournamentCategoryDto
        {
            Id = tournamentCategory.Id,
            Category = tournamentCategory.Category,
            MaxPairs = tournamentCategory.MaxPairs,
            RegistrationFee = tournamentCategory.RegistrationFee,
            ActiveRegistrationsCount = activeRegistrationsCount
        };
    }
}