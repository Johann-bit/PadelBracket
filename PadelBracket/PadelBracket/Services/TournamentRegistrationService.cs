using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class TournamentRegistrationService
{
    public TournamentRegistration RegisterPair(
        Tournament tournament,
        Pair pair,
        int category)
    {
        if (tournament == null)
            throw new ArgumentNullException(nameof(tournament));

        if (pair == null)
            throw new ArgumentNullException(nameof(pair));

        ValidateCategory(category);
        ValidateTournamentCanReceiveRegistrations(tournament);
        ValidatePairCanRegister(pair, category);

        TournamentRegistration registration = new TournamentRegistration(
            tournament.Id,
            pair,
            category);

        tournament.AddRegistration(registration);

        return registration;
    }

    public void ConfirmRegistration(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        registration.Confirm();
    }

    public void RejectRegistration(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        registration.Reject();
    }

    public void CancelRegistration(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        registration.Cancel();
    }

    public void MarkRegistrationAsPaid(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        registration.MarkAsPaid();
    }

    public void RefundRegistration(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        registration.MarkAsRefunded();
    }

    private static void ValidateTournamentCanReceiveRegistrations(Tournament tournament)
    {
        if (tournament.Status != TournamentStatus.Draft)
            throw new InvalidOperationException("Registrations are only allowed while the tournament is in draft status.");
    }

    private static void ValidatePairCanRegister(Pair pair, int category)
    {
        if (!pair.PlayerOne.HasCompleteProfile || !pair.PlayerTwo.HasCompleteProfile)
            throw new InvalidOperationException("Both players must have a complete profile before registering to a tournament.");

        if (!pair.CanPlayInCategory(category))
            throw new InvalidOperationException("Pair cannot play in this category.");
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }
}