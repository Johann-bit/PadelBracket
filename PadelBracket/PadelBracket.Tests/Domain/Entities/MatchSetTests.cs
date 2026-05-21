using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class MatchSetTests
{
    [TestMethod]
    public void Constructor_WhenRegularSetEndsSixSix_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(6, 6)
        );
    }

    [TestMethod]
    public void Constructor_WhenRegularSetWinnerHasMoreThanSevenGames_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(8, 6)
        );
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsSixFive_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(6, 5)
        );
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsSevenFour_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(7, 4)
        );
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsFiveThree_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(5, 3)
        );
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsSixFour_IsValid()
    {
        var set = new MatchSet(6, 4);

        Assert.AreEqual(6, set.PairOneScore);
        Assert.AreEqual(4, set.PairTwoScore);
        Assert.IsFalse(set.IsSuperTieBreak);
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsSevenFive_IsValid()
    {
        var set = new MatchSet(7, 5);

        Assert.AreEqual(7, set.PairOneScore);
        Assert.AreEqual(5, set.PairTwoScore);
        Assert.IsFalse(set.IsSuperTieBreak);
    }

    [TestMethod]
    public void Constructor_WhenRegularSetEndsSevenSix_IsValid()
    {
        var set = new MatchSet(7, 6);

        Assert.AreEqual(7, set.PairOneScore);
        Assert.AreEqual(6, set.PairTwoScore);
        Assert.IsFalse(set.IsSuperTieBreak);
    }

    [TestMethod]
    public void Constructor_WhenSuperTieBreakEndsElevenNine_IsValid()
    {
        var set = new MatchSet(11, 9, true);

        Assert.AreEqual(11, set.PairOneScore);
        Assert.AreEqual(9, set.PairTwoScore);
        Assert.IsTrue(set.IsSuperTieBreak);
    }

    [TestMethod]
    public void Constructor_WhenSuperTieBreakWinnerHasLessThanEleven_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(10, 8, true)
        );
    }

    [TestMethod]
    public void Constructor_WhenSuperTieBreakDifferenceIsLowerThanTwo_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new MatchSet(11, 10, true)
        );
    }
}