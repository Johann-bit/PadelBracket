using PadelBracket.Domain.Entities;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class StandingServiceTests
{
    [TestMethod]
    public void CalculateStandings_WhenGroupHasPairs_ReturnsOneStandingPerPair()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        Assert.AreEqual(2, standings.Count);
    }

    [TestMethod]
    public void CalculateStandings_WhenMatchHasResult_UpdatesPlayedWonLostAndPoints()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        var match = group.Matches.First();

        match.RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        }));

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        var winnerStanding = standings.First(s => s.Pair.Id == pairOne.Id);
        var loserStanding = standings.First(s => s.Pair.Id == pairTwo.Id);

        Assert.AreEqual(1, winnerStanding.Played);
        Assert.AreEqual(1, winnerStanding.Won);
        Assert.AreEqual(0, winnerStanding.Lost);
        Assert.AreEqual(3, winnerStanding.Points);

        Assert.AreEqual(1, loserStanding.Played);
        Assert.AreEqual(0, loserStanding.Won);
        Assert.AreEqual(1, loserStanding.Lost);
        Assert.AreEqual(0, loserStanding.Points);
    }

    [TestMethod]
    public void CalculateStandings_WhenMatchHasResult_UpdatesSetsCorrectly()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        var match = group.Matches.First();

        match.RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        }));

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        var pairOneStanding = standings.First(s => s.Pair.Id == pairOne.Id);
        var pairTwoStanding = standings.First(s => s.Pair.Id == pairTwo.Id);

        Assert.AreEqual(2, pairOneStanding.SetsFor);
        Assert.AreEqual(0, pairOneStanding.SetsAgainst);
        Assert.AreEqual(2, pairOneStanding.SetDifference);

        Assert.AreEqual(0, pairTwoStanding.SetsFor);
        Assert.AreEqual(2, pairTwoStanding.SetsAgainst);
        Assert.AreEqual(-2, pairTwoStanding.SetDifference);
    }

    [TestMethod]
    public void CalculateStandings_WhenMatchHasResult_UpdatesGamesCorrectly()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        var match = group.Matches.First();

        match.RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        }));

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        var pairOneStanding = standings.First(s => s.Pair.Id == pairOne.Id);
        var pairTwoStanding = standings.First(s => s.Pair.Id == pairTwo.Id);

        Assert.AreEqual(12, pairOneStanding.GamesFor);
        Assert.AreEqual(7, pairOneStanding.GamesAgainst);
        Assert.AreEqual(5, pairOneStanding.GameDifference);

        Assert.AreEqual(7, pairTwoStanding.GamesFor);
        Assert.AreEqual(12, pairTwoStanding.GamesAgainst);
        Assert.AreEqual(-5, pairTwoStanding.GameDifference);
    }

    [TestMethod]
    public void CalculateStandings_WhenMultipleMatchesExist_OrdersByPointsSetDifferenceAndGameDifference()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        group.Matches[0].RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        })); // pairOne beats pairTwo 2-0

        group.Matches[1].RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(4, 6),
            new MatchSet(6, 3),
            new MatchSet(11, 9, true)
        })); // pairOne beats pairThree 2-1

        group.Matches[2].RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 2),
            new MatchSet(6, 2)
        })); // pairTwo beats pairThree 2-0

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        Assert.AreEqual(pairOne.Id, standings[0].Pair.Id);
        Assert.AreEqual(pairTwo.Id, standings[1].Pair.Id);
        Assert.AreEqual(pairThree.Id, standings[2].Pair.Id);
    }

    [TestMethod]
    public void CalculateStandings_WhenMatchHasNoResult_IgnoresMatch()
    {
        var group = CreateGroup();

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        Assert.IsTrue(standings.All(s => s.Played == 0));
        Assert.IsTrue(standings.All(s => s.Points == 0));
        Assert.IsTrue(standings.All(s => s.SetsFor == 0));
        Assert.IsTrue(standings.All(s => s.SetsAgainst == 0));
        Assert.IsTrue(standings.All(s => s.GamesFor == 0));
        Assert.IsTrue(standings.All(s => s.GamesAgainst == 0));
    }

    private static Group CreateGroup()
    {
        return new Group("Group A", 5);
    }

    private static Pair CreatePair(string playerOneName, string playerTwoName)
    {
        var playerOne = new Player(playerOneName);
        var playerTwo = new Player(playerTwoName);

        return new Pair(playerOne, playerTwo);
    }
}
