using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Domain.DTOs;

public class TournamentRegistrationDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid PairId { get; set; }
    public string PairDisplayName { get; set; } = string.Empty;
    public int Category { get; set; }
    public RegistrationStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime RegisteredAt { get; set; }

    public string CategoryLabel => $"{Category}ta";

    public string StatusLabel => Status switch
    {
        RegistrationStatus.Pending => "Pendiente",
        RegistrationStatus.Confirmed => "Confirmada",
        RegistrationStatus.Rejected => "Rechazada",
        RegistrationStatus.Cancelled => "Cancelada",
        _ => Status.ToString()
    };

    public string PaymentStatusLabel => PaymentStatus switch
    {
        PaymentStatus.Pending => "Pendiente",
        PaymentStatus.Paid => "Pago",
        PaymentStatus.Refunded => "Reembolsado",
        PaymentStatus.Cancelled => "Cancelado",
        _ => PaymentStatus.ToString()
    };

    public static TournamentRegistrationDto FromEntity(TournamentRegistration registration)
    {
        return new TournamentRegistrationDto
        {
            Id = registration.Id,
            TournamentId = registration.TournamentId,
            PairId = registration.Pair.Id,
            PairDisplayName = registration.Pair.DisplayName,
            Category = registration.Category,
            Status = registration.Status,
            PaymentStatus = registration.PaymentStatus,
            RegisteredAt = registration.RegisteredAt
        };
    }
}