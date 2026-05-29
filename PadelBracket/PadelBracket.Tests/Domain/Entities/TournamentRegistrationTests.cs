using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class TournamentRegistrationTests
{
    [TestMethod]
    public void Constructor_WithValidData_ShouldCreateRegistration()
    {
        Guid tournamentId = Guid.NewGuid();
        Pair pair = CreateVerifiedPair(6);

        TournamentRegistration registration = new TournamentRegistration(tournamentId, pair, 6);

        Assert.AreNotEqual(Guid.Empty, registration.Id);
        Assert.AreEqual(tournamentId, registration.TournamentId);
        Assert.AreEqual(pair, registration.Pair);
        Assert.AreEqual(6, registration.Category);
        Assert.AreEqual(RegistrationStatus.Pending, registration.Status);
        Assert.AreEqual(PaymentStatus.Pending, registration.PaymentStatus);
        Assert.AreNotEqual(default, registration.RegisteredAt);
    }

    [TestMethod]
    public void Constructor_WithEmptyTournamentId_ShouldThrowArgumentException()
    {
        Pair pair = CreateVerifiedPair(6);

        Assert.ThrowsException<ArgumentException>(() =>
            new TournamentRegistration(Guid.Empty, pair, 6));
    }

    [TestMethod]
    public void Constructor_WithNullPair_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new TournamentRegistration(Guid.NewGuid(), null!, 6));
    }

    [TestMethod]
    public void Constructor_WithCategoryLowerThanOne_ShouldThrowArgumentException()
    {
        Pair pair = CreateVerifiedPair(6);

        Assert.ThrowsException<ArgumentException>(() =>
            new TournamentRegistration(Guid.NewGuid(), pair, 0));
    }

    [TestMethod]
    public void Constructor_WithCategoryGreaterThanEight_ShouldThrowArgumentException()
    {
        Pair pair = CreateVerifiedPair(6);

        Assert.ThrowsException<ArgumentException>(() =>
            new TournamentRegistration(Guid.NewGuid(), pair, 9));
    }

    [TestMethod]
    public void Confirm_WhenPending_ShouldSetStatusToConfirmed()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Confirm();

        Assert.AreEqual(RegistrationStatus.Confirmed, registration.Status);
    }

    [TestMethod]
    public void Confirm_WhenCancelled_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.ThrowsException<InvalidOperationException>(() => registration.Confirm());
    }

    [TestMethod]
    public void Confirm_WhenRejected_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Reject();

        Assert.ThrowsException<InvalidOperationException>(() => registration.Confirm());
    }

    [TestMethod]
    public void Reject_WhenPending_ShouldSetStatusToRejected()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Reject();

        Assert.AreEqual(RegistrationStatus.Rejected, registration.Status);
    }

    [TestMethod]
    public void Reject_WhenCancelled_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.ThrowsException<InvalidOperationException>(() => registration.Reject());
    }

    [TestMethod]
    public void Reject_WhenConfirmed_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Confirm();

        Assert.ThrowsException<InvalidOperationException>(() => registration.Reject());
    }

    [TestMethod]
    public void Cancel_WhenPending_ShouldSetStatusAndPaymentStatusToCancelled()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.AreEqual(RegistrationStatus.Cancelled, registration.Status);
        Assert.AreEqual(PaymentStatus.Cancelled, registration.PaymentStatus);
    }

    [TestMethod]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.ThrowsException<InvalidOperationException>(() => registration.Cancel());
    }

    [TestMethod]
    public void MarkAsPaid_WhenPending_ShouldSetPaymentStatusToPaid()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.MarkAsPaid();

        Assert.AreEqual(PaymentStatus.Paid, registration.PaymentStatus);
    }

    [TestMethod]
    public void MarkAsPaid_WhenCancelled_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.ThrowsException<InvalidOperationException>(() => registration.MarkAsPaid());
    }

    [TestMethod]
    public void MarkAsPaid_WhenRejected_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Reject();

        Assert.ThrowsException<InvalidOperationException>(() => registration.MarkAsPaid());
    }

    [TestMethod]
    public void MarkAsRefunded_WhenPaid_ShouldSetPaymentStatusToRefunded()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.MarkAsPaid();
        registration.MarkAsRefunded();

        Assert.AreEqual(PaymentStatus.Refunded, registration.PaymentStatus);
    }

    [TestMethod]
    public void MarkAsRefunded_WhenNotPaid_ShouldThrowInvalidOperationException()
    {
        TournamentRegistration registration = CreateRegistration();

        Assert.ThrowsException<InvalidOperationException>(() => registration.MarkAsRefunded());
    }

    [TestMethod]
    public void IsActive_WhenPending_ShouldReturnTrue()
    {
        TournamentRegistration registration = CreateRegistration();

        Assert.IsTrue(registration.IsActive());
    }

    [TestMethod]
    public void IsActive_WhenConfirmed_ShouldReturnTrue()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Confirm();

        Assert.IsTrue(registration.IsActive());
    }

    [TestMethod]
    public void IsActive_WhenRejected_ShouldReturnFalse()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Reject();

        Assert.IsFalse(registration.IsActive());
    }

    [TestMethod]
    public void IsActive_WhenCancelled_ShouldReturnFalse()
    {
        TournamentRegistration registration = CreateRegistration();

        registration.Cancel();

        Assert.IsFalse(registration.IsActive());
    }

    private static TournamentRegistration CreateRegistration()
    {
        return new TournamentRegistration(Guid.NewGuid(), CreateVerifiedPair(6), 6);
    }

    private static Pair CreateVerifiedPair(int category)
    {
        Player playerOne = CreatePlayer("Johann", "johann@mail.com", category);
        Player playerTwo = CreatePlayer("Franco", "franco@mail.com", category);

        playerOne.Verify();
        playerTwo.Verify();

        return new Pair(playerOne, playerTwo);
    }

    private static Player CreatePlayer(string name, string email, int category)
    {
        return new Player(
            name,
            email,
            DominantHand.Right,
            PreferredSide.Drive,
            category);
    }
}