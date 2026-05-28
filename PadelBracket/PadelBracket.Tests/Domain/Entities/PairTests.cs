using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class PairTests
{
    [TestMethod]
    public void Constructor_WithTwoDifferentPlayers_ShouldCreatePair()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");

        Pair pair = new Pair(playerOne, playerTwo);

        Assert.AreNotEqual(Guid.Empty, pair.Id);
        Assert.AreEqual(playerOne, pair.PlayerOne);
        Assert.AreEqual(playerTwo, pair.PlayerTwo);
        Assert.IsNull(pair.Category);
    }

    [TestMethod]
    public void Constructor_WithTwoDifferentPlayersWithCategories_ShouldCreatePairWithCategory()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 5);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);

        Pair pair = new Pair(playerOne, playerTwo);

        Assert.AreEqual(5, pair.Category);
    }

    [TestMethod]
    public void Constructor_WithNullPlayerOne_ShouldThrowArgumentNullException()
    {
        Player playerTwo = new Player("Franco");

        Assert.ThrowsException<ArgumentNullException>(() => new Pair(null!, playerTwo));
    }

    [TestMethod]
    public void Constructor_WithNullPlayerTwo_ShouldThrowArgumentNullException()
    {
        Player playerOne = new Player("Johann");

        Assert.ThrowsException<ArgumentNullException>(() => new Pair(playerOne, null!));
    }

    [TestMethod]
    public void Constructor_WithSamePlayerTwice_ShouldThrowArgumentException()
    {
        Player player = new Player("Johann");

        Assert.ThrowsException<ArgumentException>(() => new Pair(player, player));
    }

    [TestMethod]
    public void DisplayName_ShouldReturnPlayerOneAndPlayerTwoNames()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        string displayName = pair.DisplayName;

        Assert.AreEqual("Johann / Franco", displayName);
    }

    [TestMethod]
    public void RenamePlayers_WithValidNames_ShouldRenameBothPlayers()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        pair.RenamePlayers("Bruno", "Joshua");

        Assert.AreEqual("Bruno", pair.PlayerOne.Name);
        Assert.AreEqual("Joshua", pair.PlayerTwo.Name);
        Assert.AreEqual("Bruno / Joshua", pair.DisplayName);
    }

    [TestMethod]
    public void RenamePlayers_WithNamesContainingExtraSpaces_ShouldTrimBothNames()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        pair.RenamePlayers("  Bruno  ", "  Joshua  ");

        Assert.AreEqual("Bruno", pair.PlayerOne.Name);
        Assert.AreEqual("Joshua", pair.PlayerTwo.Name);
        Assert.AreEqual("Bruno / Joshua", pair.DisplayName);
    }

    [TestMethod]
    public void RenamePlayers_WithNullPlayerOneName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);
        string? playerOneName = null;

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers(playerOneName!, "Joshua"));
    }

    [TestMethod]
    public void RenamePlayers_WithNullPlayerTwoName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);
        string? playerTwoName = null;

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers("Bruno", playerTwoName!));
    }

    [TestMethod]
    public void RenamePlayers_WithEmptyPlayerOneName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers("", "Joshua"));
    }

    [TestMethod]
    public void RenamePlayers_WithEmptyPlayerTwoName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers("Bruno", ""));
    }

    [TestMethod]
    public void RenamePlayers_WithWhiteSpacePlayerOneName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers("   ", "Joshua"));
    }

    [TestMethod]
    public void RenamePlayers_WithWhiteSpacePlayerTwoName_ShouldThrowArgumentException()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.ThrowsException<ArgumentException>(() => pair.RenamePlayers("Bruno", "   "));
    }

    [TestMethod]
    public void ContainsPlayer_WithExistingPlayer_ShouldReturnTrue()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsTrue(pair.ContainsPlayer(playerOne.Id));
        Assert.IsTrue(pair.ContainsPlayer(playerTwo.Id));
    }

    [TestMethod]
    public void ContainsPlayer_WithUnknownPlayer_ShouldReturnFalse()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsFalse(pair.ContainsPlayer(Guid.NewGuid()));
    }

    [TestMethod]
    public void HasSamePlayersAs_WithSamePlayersInDifferentOrder_ShouldReturnTrue()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerTwo, playerOne);

        Assert.IsTrue(pairOne.HasSamePlayersAs(pairTwo));
    }

    [TestMethod]
    public void CanPlayInCategory_WithSameCategory_ShouldReturnTrue()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsTrue(pair.CanPlayInCategory(6));
    }

    [TestMethod]
    public void CanPlayInCategory_WithLowerLevelTournamentCategory_ShouldReturnTrue()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsTrue(pair.CanPlayInCategory(7));
    }

    [TestMethod]
    public void CanPlayInCategory_WithHigherLevelTournamentCategory_ShouldReturnFalse()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsFalse(pair.CanPlayInCategory(5));
    }

    [TestMethod]
    public void CanPlayInCategory_WithPairWithoutCategory_ShouldReturnFalse()
    {
        Player playerOne = new Player("Johann");
        Player playerTwo = new Player("Franco");
        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsFalse(pair.CanPlayInCategory(6));
    }

    [TestMethod]
    public void IsFullyVerified_WithBothPlayersVerified_ShouldReturnTrue()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);

        playerOne.Verify();
        playerTwo.Verify();

        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsTrue(pair.IsFullyVerified());
    }

    [TestMethod]
    public void IsFullyVerified_WithOnePlayerNotVerified_ShouldReturnFalse()
    {
        Player playerOne = CreateCompletePlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Franco", "franco@mail.com", 6);

        playerOne.Verify();

        Pair pair = new Pair(playerOne, playerTwo);

        Assert.IsFalse(pair.IsFullyVerified());
    }

    private static Player CreateCompletePlayer(string name, string email, int category)
    {
        return new Player(
            name,
            email,
            DominantHand.Right,
            PreferredSide.Drive,
            category);
    }
}