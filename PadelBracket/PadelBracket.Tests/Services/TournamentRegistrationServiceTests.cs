using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class TournamentRegistrationServiceTests
{
    [TestMethod]
    public void RegisterPair_WithValidData_ShouldCreateRegistration()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        TournamentRegistration registration = service.RegisterPair(tournament, pair, 6);

        Assert.AreNotEqual(Guid.Empty, registration.Id);
        Assert.AreEqual(tournament.Id, registration.TournamentId);
        Assert.AreEqual(pair, registration.Pair);
        Assert.AreEqual(6, registration.Category);
        Assert.AreEqual(RegistrationStatus.Pending, registration.Status);
        Assert.AreEqual(PaymentStatus.Pending, registration.PaymentStatus);
        Assert.AreEqual(1, tournament.Registrations.Count);
    }

    [TestMethod]
    public void RegisterPair_WithNullTournament_ShouldThrowArgumentNullException()
    {
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<ArgumentNullException>(() =>
            service.RegisterPair(null!, pair, 6));
    }

    [TestMethod]
    public void RegisterPair_WithNullPair_ShouldThrowArgumentNullException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<ArgumentNullException>(() =>
            service.RegisterPair(tournament, null!, 6));
    }

    [TestMethod]
    public void RegisterPair_WithInvalidCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.RegisterPair(tournament, pair, 0));
    }

    [TestMethod]
    public void RegisterPair_WhenTournamentIsNotDraft_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        tournament.StartGroupStage();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.RegisterPair(tournament, pair, 6));
    }

    [TestMethod]
    public void RegisterPair_WithCompleteUnverifiedPair_ShouldCreateRegistration()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateUnverifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        TournamentRegistration registration = service.RegisterPair(tournament, pair, 6);

        Assert.AreNotEqual(Guid.Empty, registration.Id);
        Assert.AreEqual(tournament.Id, registration.TournamentId);
        Assert.AreEqual(pair, registration.Pair);
        Assert.AreEqual(6, registration.Category);
        Assert.AreEqual(1, tournament.Registrations.Count);
    }

    [TestMethod]
    public void RegisterPair_WithIncompletePairProfile_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);

        Player playerOne = new Player("Johann");
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);

        Pair pair = new Pair(playerOne, playerTwo);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.RegisterPair(tournament, pair, 6));
    }

    [TestMethod]
    public void RegisterPair_WithPairFromHigherLevelThanTournamentCategory_ShouldRegister()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 5);
        TournamentRegistrationService service = new TournamentRegistrationService();

        TournamentRegistration registration = service.RegisterPair(tournament, pair, 6);

        Assert.AreEqual(6, registration.Category);
        Assert.AreEqual(1, tournament.Registrations.Count);
    }

    [TestMethod]
    public void RegisterPair_WithPairFromLowerLevelThanTournamentCategory_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = CreateTournamentWithCategory(5, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.RegisterPair(tournament, pair, 5));
    }

    [TestMethod]
    public void RegisterPair_WithUnknownTournamentCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.RegisterPair(tournament, pair, 7));
    }

    [TestMethod]
    public void RegisterPair_WhenCategoryHasNoAvailableSlots_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 1);
        Pair pairOne = CreateVerifiedPair("Johann", "Franco", 6);
        Pair pairTwo = CreateVerifiedPair("Bruno", "Joshua", 6);
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.RegisterPair(tournament, pairOne, 6);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.RegisterPair(tournament, pairTwo, 6));
    }

    [TestMethod]
    public void RegisterPair_WithSamePairTwice_ShouldThrowArgumentException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);

        Player playerOne = CreateVerifiedPlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerOne, playerTwo);

        TournamentRegistrationService service = new TournamentRegistrationService();

        service.RegisterPair(tournament, pairOne, 6);

        Assert.ThrowsException<ArgumentException>(() =>
            service.RegisterPair(tournament, pairTwo, 6));
    }

    [TestMethod]
    public void RegisterPair_WithPlayerAlreadyRegisteredInAnotherPair_ShouldThrowArgumentException()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);

        Player playerOne = CreateVerifiedPlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);
        Player playerThree = CreateVerifiedPlayer("Bruno", "bruno@mail.com", 6);

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerOne, playerThree);

        TournamentRegistrationService service = new TournamentRegistrationService();

        service.RegisterPair(tournament, pairOne, 6);

        Assert.ThrowsException<ArgumentException>(() =>
            service.RegisterPair(tournament, pairTwo, 6));
    }

    [TestMethod]
    public void ConfirmRegistration_WithValidRegistration_ShouldConfirm()
    {
        TournamentRegistration registration = CreateRegistration();
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.ConfirmRegistration(registration);

        Assert.AreEqual(RegistrationStatus.Confirmed, registration.Status);
    }

    [TestMethod]
    public void RejectRegistration_WithValidRegistration_ShouldReject()
    {
        TournamentRegistration registration = CreateRegistration();
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.RejectRegistration(registration);

        Assert.AreEqual(RegistrationStatus.Rejected, registration.Status);
    }

    [TestMethod]
    public void CancelRegistration_WithValidRegistration_ShouldCancel()
    {
        TournamentRegistration registration = CreateRegistration();
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.CancelRegistration(registration);

        Assert.AreEqual(RegistrationStatus.Cancelled, registration.Status);
        Assert.AreEqual(PaymentStatus.Cancelled, registration.PaymentStatus);
    }

    [TestMethod]
    public void MarkRegistrationAsPaid_WithValidRegistration_ShouldMarkAsPaid()
    {
        TournamentRegistration registration = CreateRegistration();
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.MarkRegistrationAsPaid(registration);

        Assert.AreEqual(PaymentStatus.Paid, registration.PaymentStatus);
    }

    [TestMethod]
    public void RefundRegistration_WithPaidRegistration_ShouldRefund()
    {
        TournamentRegistration registration = CreateRegistration();
        TournamentRegistrationService service = new TournamentRegistrationService();

        service.MarkRegistrationAsPaid(registration);
        service.RefundRegistration(registration);

        Assert.AreEqual(PaymentStatus.Refunded, registration.PaymentStatus);
    }

    private static Tournament CreateTournamentWithCategory(int category, int maxPairs)
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(category, maxPairs, 800));

        return tournament;
    }

    private static TournamentRegistration CreateRegistration()
    {
        Tournament tournament = CreateTournamentWithCategory(6, 16);
        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);

        return new TournamentRegistration(tournament.Id, pair, 6);
    }

    private static Pair CreateVerifiedPair(string playerOneName, string playerTwoName, int category)
    {
        Player playerOne = CreateVerifiedPlayer(
            playerOneName,
            $"{playerOneName.ToLower()}@mail.com",
            category);

        Player playerTwo = CreateVerifiedPlayer(
            playerTwoName,
            $"{playerTwoName.ToLower()}@mail.com",
            category);

        return new Pair(playerOne, playerTwo);
    }

    private static Pair CreateUnverifiedPair(string playerOneName, string playerTwoName, int category)
    {
        Player playerOne = new Player(
            playerOneName,
            $"{playerOneName.ToLower()}@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            category);

        Player playerTwo = new Player(
            playerTwoName,
            $"{playerTwoName.ToLower()}@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            category);

        return new Pair(playerOne, playerTwo);
    }

    private static Player CreateVerifiedPlayer(string name, string email, int category)
    {
        Player player = new Player(
            name,
            email,
            DominantHand.Right,
            PreferredSide.Drive,
            category);

        player.Verify();

        return player;
    }
}