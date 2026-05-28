using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void Constructor_WithValidData_ShouldCreatePlayer()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.AreNotEqual(Guid.Empty, player.Id);
        Assert.AreEqual("Juan Perez", player.Name);
        Assert.AreEqual("juan@mail.com", player.Email);
        Assert.AreEqual(DominantHand.Right, player.DominantHand);
        Assert.AreEqual(PreferredSide.Drive, player.PreferredSide);
        Assert.AreEqual(6, player.Category);
        Assert.AreEqual(PlayerVerificationStatus.Pending, player.VerificationStatus);
        Assert.AreEqual(500, player.RankingPoints);
    }

    [TestMethod]
    public void Constructor_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Player(
                "",
                "juan@mail.com",
                DominantHand.Right,
                PreferredSide.Drive,
                6));
    }

    [TestMethod]
    public void Constructor_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Player(
                "Juan Perez",
                "",
                DominantHand.Right,
                PreferredSide.Drive,
                6));
    }

    [TestMethod]
    public void Constructor_WithInvalidEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Player(
                "Juan Perez",
                "juanmail.com",
                DominantHand.Right,
                PreferredSide.Drive,
                6));
    }

    [TestMethod]
    public void Constructor_WithCategoryLowerThanOne_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Player(
                "Juan Perez",
                "juan@mail.com",
                DominantHand.Right,
                PreferredSide.Drive,
                0));
    }

    [TestMethod]
    public void Constructor_WithCategoryGreaterThanEight_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Player(
                "Juan Perez",
                "juan@mail.com",
                DominantHand.Right,
                PreferredSide.Drive,
                9));
    }

    [TestMethod]
    public void Verify_ShouldSetVerificationStatusToVerified()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        player.Verify();

        Assert.AreEqual(PlayerVerificationStatus.Verified, player.VerificationStatus);
        Assert.IsTrue(player.IsVerified);
    }

    [TestMethod]
    public void RejectVerification_ShouldSetVerificationStatusToRejected()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        player.RejectVerification();

        Assert.AreEqual(PlayerVerificationStatus.Rejected, player.VerificationStatus);
        Assert.IsFalse(player.IsVerified);
    }

    [TestMethod]
    public void ChangeCategory_ShouldUpdateCategoryRankingPointsAndSetVerificationPending()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        player.Verify();

        player.ChangeCategory(5);

        Assert.AreEqual(5, player.Category);
        Assert.AreEqual(850, player.RankingPoints);
        Assert.AreEqual(PlayerVerificationStatus.Pending, player.VerificationStatus);
    }

    [TestMethod]
    public void AddRankingPoints_WithValidPoints_ShouldIncreaseRankingPoints()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        player.AddRankingPoints(100);

        Assert.AreEqual(600, player.RankingPoints);
    }

    [TestMethod]
    public void AddRankingPoints_WithInvalidPoints_ShouldThrowException()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.ThrowsException<ArgumentException>(() => player.AddRankingPoints(0));
    }

    [TestMethod]
    public void RemoveRankingPoints_WithValidPoints_ShouldDecreaseRankingPoints()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        player.RemoveRankingPoints(100);

        Assert.AreEqual(400, player.RankingPoints);
    }

    [TestMethod]
    public void RemoveRankingPoints_ShouldNotSetRankingPointsBelowZero()
    {
        Player player = new Player(
            "Juan Perez",
            "juan@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            8);

        player.RemoveRankingPoints(500);

        Assert.AreEqual(0, player.RankingPoints);
    }
}