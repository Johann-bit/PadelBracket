using PadelBracket.Domain.Enums;

namespace PadelBracket.Domain.Entities;

public class TournamentRegistration
{
    public Guid Id { get; private set; }
    public Guid TournamentId { get; private set; }
    public Pair Pair { get; private set; }
    public int Category { get; private set; }
    public RegistrationStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    private TournamentRegistration()
    {
        Pair = null!;
    }

    public TournamentRegistration(Guid tournamentId, Pair pair, int category)
    {
        if (tournamentId == Guid.Empty)
            throw new ArgumentException("Tournament id is required.");

        Pair = pair ?? throw new ArgumentNullException(nameof(pair));

        ValidateCategory(category);

        Id = Guid.NewGuid();
        TournamentId = tournamentId;
        Category = category;
        Status = RegistrationStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
        RegisteredAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status == RegistrationStatus.Cancelled)
            throw new InvalidOperationException("Cancelled registration cannot be confirmed.");

        if (Status == RegistrationStatus.Rejected)
            throw new InvalidOperationException("Rejected registration cannot be confirmed.");

        Status = RegistrationStatus.Confirmed;
    }

    public void Reject()
    {
        if (Status == RegistrationStatus.Cancelled)
            throw new InvalidOperationException("Cancelled registration cannot be rejected.");

        if (Status == RegistrationStatus.Confirmed)
            throw new InvalidOperationException("Confirmed registration cannot be rejected.");

        Status = RegistrationStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status == RegistrationStatus.Cancelled)
            throw new InvalidOperationException("Registration is already cancelled.");

        Status = RegistrationStatus.Cancelled;
        PaymentStatus = PaymentStatus.Cancelled;
    }

    public void MarkAsPaid()
    {
        if (Status == RegistrationStatus.Cancelled)
            throw new InvalidOperationException("Cancelled registration cannot be paid.");

        if (Status == RegistrationStatus.Rejected)
            throw new InvalidOperationException("Rejected registration cannot be paid.");

        PaymentStatus = PaymentStatus.Paid;
    }

    public void MarkAsRefunded()
    {
        if (PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException("Only paid registrations can be refunded.");

        PaymentStatus = PaymentStatus.Refunded;
    }

    public bool IsActive()
    {
        return Status == RegistrationStatus.Pending ||
               Status == RegistrationStatus.Confirmed;
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }
}