using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class MatchTests
{
    [TestMethod]
    public void Constructor_WithTwoDifferentPairs_ShouldCreateMatch()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");

        Match match = new Match(pairOne, pairTwo);

        Assert.AreNotEqual(Guid.Empty, match.Id);
        Assert.AreEqual(pairOne, match.PairOne);
        Assert.AreEqual(pairTwo, match.PairTwo);
        Assert.IsFalse(match.HasResult);
        Assert.IsNull(match.Result);
        Assert.IsNull(match.Winner);
        Assert.IsNull(match.Loser);
    }

    [TestMethod]
    public void Constructor_WithNullPairOne_ShouldThrowArgumentNullException()
    {
        Pair pairTwo = CreatePair("Bruno", "Joshua");

        Assert.ThrowsException<ArgumentNullException>(() => new Match(null!, pairTwo));
    }

    [TestMethod]
    public void Constructor_WithNullPairTwo_ShouldThrowArgumentNullException()
    {
        Pair pairOne = CreatePair("Johann", "Franco");

        Assert.ThrowsException<ArgumentNullException>(() => new Match(pairOne, null!));
    }

    [TestMethod]
    public void Constructor_WithSamePairTwice_ShouldThrowArgumentException()
    {
        Pair pair = CreatePair("Johann", "Franco");

        Assert.ThrowsException<ArgumentException>(() => new Match(pair, pair));
    }

    [TestMethod]
    public void RegisterResult_WithValidResult_ShouldSetResult()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);

        MatchResult result = CreatePairOneWinsResult();

        match.RegisterResult(result);

        Assert.AreEqual(result, match.Result);
        Assert.IsTrue(match.HasResult);
    }

    [TestMethod]
    public void RegisterResult_WithNullResult_ShouldThrowArgumentNullException()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);

        Assert.ThrowsException<ArgumentNullException>(() => match.RegisterResult(null!));
    }

    [TestMethod]
    public void ClearResult_WhenMatchHasResult_ShouldRemoveResult()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);
        MatchResult result = CreatePairOneWinsResult();

        match.RegisterResult(result);
        match.ClearResult();

        Assert.IsNull(match.Result);
        Assert.IsFalse(match.HasResult);
        Assert.IsNull(match.Winner);
        Assert.IsNull(match.Loser);
    }

    [TestMethod]
    public void Winner_WhenResultIsNull_ShouldReturnNull()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);

        Pair? winner = match.Winner;

        Assert.IsNull(winner);
    }

    [TestMethod]
    public void Loser_WhenResultIsNull_ShouldReturnNull()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);

        Pair? loser = match.Loser;

        Assert.IsNull(loser);
    }

    [TestMethod]
    public void Winner_WhenPairOneWins_ShouldReturnPairOne()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);
        MatchResult result = CreatePairOneWinsResult();

        match.RegisterResult(result);

        Assert.AreEqual(pairOne, match.Winner);
    }

    [TestMethod]
    public void Loser_WhenPairOneWins_ShouldReturnPairTwo()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);
        MatchResult result = CreatePairOneWinsResult();

        match.RegisterResult(result);

        Assert.AreEqual(pairTwo, match.Loser);
    }

    [TestMethod]
    public void Winner_WhenPairTwoWins_ShouldReturnPairTwo()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);
        MatchResult result = CreatePairTwoWinsResult();

        match.RegisterResult(result);

        Assert.AreEqual(pairTwo, match.Winner);
    }

    [TestMethod]
    public void Loser_WhenPairTwoWins_ShouldReturnPairOne()
    {
        Pair pairOne = CreatePair("Johann", "Franco");
        Pair pairTwo = CreatePair("Bruno", "Joshua");
        Match match = new Match(pairOne, pairTwo);
        MatchResult result = CreatePairTwoWinsResult();

        match.RegisterResult(result);

        Assert.AreEqual(pairOne, match.Loser);
    }

    private static Pair CreatePair(string playerOneName, string playerTwoName)
    {
        Player playerOne = new Player(playerOneName);
        Player playerTwo = new Player(playerTwoName);

        return new Pair(playerOne, playerTwo);
    }

    private static MatchResult CreatePairOneWinsResult()
    {
        return new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 4),
            new MatchSet(6, 3)
        });
    }

    private static MatchResult CreatePairTwoWinsResult()
    {
        return new MatchResult(new List<MatchSet>
        {
            new MatchSet(4, 6),
            new MatchSet(3, 6)
        });
    }
}