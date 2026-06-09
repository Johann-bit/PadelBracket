using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Domain.DTOs;

public class TournamentRegistrationDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid PairId { get; set; }
    public Guid PlayerOneId { get; set; }
    public Guid PlayerTwoId { get; set; }
    public string PairDisplayName { get; set; } = string.Empty;
    public int Category { get; set; }
    public RegistrationStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime RegisteredAt { get; set; }

    public string CategoryLabel => Category switch
    {
        1 => "1ra",
        2 => "2da",
        3 => "3ra",
        4 => "4ta",
        5 => "5ta",
        6 => "6ta",
        7 => "7ma",
        8 => "8va",
        _ => $"{Category}ta"
    };

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

    public string PaymentHelpText => PaymentStatus switch
    {
        PaymentStatus.Pending => "Por ahora el pago se coordina por fuera de la plataforma. El organizador lo confirma cuando lo reciba.",
        PaymentStatus.Paid => "El organizador marcó esta inscripción como paga.",
        PaymentStatus.Refunded => "El pago fue marcado como reembolsado.",
        PaymentStatus.Cancelled => "El pago quedó cancelado junto con la inscripción.",
        _ => string.Empty
    };

    public bool BelongsToPlayer(Guid playerId)
    {
        return PlayerOneId == playerId || PlayerTwoId == playerId;
    }

    public static TournamentRegistrationDto FromEntity(TournamentRegistration registration)
    {
        return new TournamentRegistrationDto
        {
            Id = registration.Id,
            TournamentId = registration.TournamentId,
            PairId = registration.Pair.Id,
            PlayerOneId = registration.Pair.PlayerOne.Id,
            PlayerTwoId = registration.Pair.PlayerTwo.Id,
            PairDisplayName = registration.Pair.DisplayName,
            Category = registration.Category,
            Status = registration.Status,
            PaymentStatus = registration.PaymentStatus,
            RegisteredAt = registration.RegisteredAt
        };
    }
}
