using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class TournamentCategoryTests
{
    [TestMethod]
    public void Constructor_WithValidData_ShouldCreateTournamentCategory()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        Assert.AreNotEqual(Guid.Empty, tournamentCategory.Id);
        Assert.AreEqual(6, tournamentCategory.Category);
        Assert.AreEqual(16, tournamentCategory.MaxPairs);
        Assert.AreEqual(800, tournamentCategory.RegistrationFee);
    }

    [TestMethod]
    public void Constructor_WithCategoryLowerThanOne_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new TournamentCategory(0, 16, 800));
    }

    [TestMethod]
    public void Constructor_WithCategoryGreaterThanEight_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new TournamentCategory(9, 16, 800));
    }

    [TestMethod]
    public void Constructor_WithMaxPairsLowerThanOne_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new TournamentCategory(6, 0, 800));
    }

    [TestMethod]
    public void Constructor_WithNegativeRegistrationFee_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new TournamentCategory(6, 16, -1));
    }

    [TestMethod]
    public void UpdateMaxPairs_WithValidValue_ShouldUpdateMaxPairs()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        tournamentCategory.UpdateMaxPairs(24);

        Assert.AreEqual(24, tournamentCategory.MaxPairs);
    }

    [TestMethod]
    public void UpdateMaxPairs_WithInvalidValue_ShouldThrowArgumentException()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        Assert.ThrowsException<ArgumentException>(() => tournamentCategory.UpdateMaxPairs(0));
    }

    [TestMethod]
    public void UpdateRegistrationFee_WithValidValue_ShouldUpdateRegistrationFee()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        tournamentCategory.UpdateRegistrationFee(1000);

        Assert.AreEqual(1000, tournamentCategory.RegistrationFee);
    }

    [TestMethod]
    public void UpdateRegistrationFee_WithNegativeValue_ShouldThrowArgumentException()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        Assert.ThrowsException<ArgumentException>(() => tournamentCategory.UpdateRegistrationFee(-1));
    }

    [TestMethod]
    public void HasAvailableSlot_WhenCurrentPairsCountIsLowerThanMaxPairs_ShouldReturnTrue()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        bool result = tournamentCategory.HasAvailableSlot(15);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasAvailableSlot_WhenCurrentPairsCountIsEqualToMaxPairs_ShouldReturnFalse()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        bool result = tournamentCategory.HasAvailableSlot(16);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasAvailableSlot_WhenCurrentPairsCountIsGreaterThanMaxPairs_ShouldReturnFalse()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        bool result = tournamentCategory.HasAvailableSlot(17);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasAvailableSlot_WithNegativeCurrentPairsCount_ShouldThrowArgumentException()
    {
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        Assert.ThrowsException<ArgumentException>(() => tournamentCategory.HasAvailableSlot(-1));
    }
}