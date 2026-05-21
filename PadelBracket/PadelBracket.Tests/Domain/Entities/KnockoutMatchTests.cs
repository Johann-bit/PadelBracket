using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class KnockoutMatchTests
{
    [TestMethod]
    public void Constructor_WhenRoundNameIsEmpty_ThrowsArgumentException()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        Assert.ThrowsException<ArgumentException>(() =>
            new KnockoutMatch("", pairOne, pairTwo)
        );
    }

    [TestMethod]
    public void Constructor_WhenDataIsValid_CreatesKnockoutMatch()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Semifinal", pairOne, pairTwo);

        Assert.AreNotEqual(Guid.Empty, match.Id);
        Assert.AreEqual("Semifinal", match.RoundName);
        Assert.AreEqual(pairOne.Id, match.PairOne!.Id);
        Assert.AreEqual(pairTwo.Id, match.PairTwo!.Id);
    }

    [TestMethod]
    public void IsReadyToPlay_WhenBothPairsAreAssigned_ReturnsTrue()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Semifinal", pairOne, pairTwo);

        Assert.IsTrue(match.IsReadyToPlay);
    }

    [TestMethod]
    public void IsReadyToPlay_WhenOnePairIsMissing_ReturnsFalse()
    {
        var pairOne = CreatePair("Juan", "Pedro");

        var match = new KnockoutMatch("Final", pairOne, null);

        Assert.IsFalse(match.IsReadyToPlay);
    }

    [TestMethod]
    public void RegisterResult_WhenMatchIsNotReady_ThrowsInvalidOperationException()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var match = new KnockoutMatch("Final", pairOne, null);

        var result = CreatePairOneWinResult();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.RegisterResult(result)
        );
    }

    [TestMethod]
    public void RegisterResult_WhenMatchIsReady_SavesResult()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Semifinal", pairOne, pairTwo);

        match.RegisterResult(CreatePairOneWinResult());

        Assert.IsTrue(match.HasResult);
        Assert.IsNotNull(match.Result);
    }

    [TestMethod]
    public void Winner_WhenPairOneWins_ReturnsPairOne()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Semifinal", pairOne, pairTwo);

        match.RegisterResult(CreatePairOneWinResult());

        Assert.IsNotNull(match.Winner);
        Assert.AreEqual(pairOne.Id, match.Winner!.Id);
    }

    [TestMethod]
    public void Loser_WhenPairOneWins_ReturnsPairTwo()
    {
        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Semifinal", pairOne, pairTwo);

        match.RegisterResult(CreatePairOneWinResult());

        Assert.IsNotNull(match.Loser);
        Assert.AreEqual(pairTwo.Id, match.Loser!.Id);
    }

    [TestMethod]
    public void AssignPairOne_WhenPairIsValid_AssignsPairOne()
    {
        var pairOne = CreatePair("Juan", "Pedro");

        var match = new KnockoutMatch("Final", null, null);

        match.AssignPairOne(pairOne);

        Assert.IsNotNull(match.PairOne);
        Assert.AreEqual(pairOne.Id, match.PairOne!.Id);
    }

    [TestMethod]
    public void AssignPairTwo_WhenPairIsValid_AssignsPairTwo()
    {
        var pairTwo = CreatePair("Nico", "Santi");

        var match = new KnockoutMatch("Final", null, null);

        match.AssignPairTwo(pairTwo);

        Assert.IsNotNull(match.PairTwo);
        Assert.AreEqual(pairTwo.Id, match.PairTwo!.Id);
    }

    private static Pair CreatePair(string playerOneName, string playerTwoName)
    {
        var playerOne = new Player(playerOneName);
        var playerTwo = new Player(playerTwoName);

        return new Pair(playerOne, playerTwo);
    }

    private static MatchResult CreatePairOneWinResult()
    {
        return new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        });
    }
}