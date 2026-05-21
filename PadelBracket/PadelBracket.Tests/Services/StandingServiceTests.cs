using PadelBracket.Domain.Entities;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class StandingServiceTests
{
    [TestMethod]
    public void CalculateStandings_WhenGroupHasPairs_ReturnsOneStandingPerPair()
    {
        var group = new Group("Group A");

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
        var group = new Group("Group A");

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.GenerateMatches();

        var match = group.Matches.First();
        match.RegisterResult(new MatchResult(6, 3));

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
    public void CalculateStandings_WhenMatchHasResult_UpdatesGamesCorrectly()
    {
        var group = new Group("Group A");

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.GenerateMatches();

        var match = group.Matches.First();
        match.RegisterResult(new MatchResult(6, 3));

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        var pairOneStanding = standings.First(s => s.Pair.Id == pairOne.Id);
        var pairTwoStanding = standings.First(s => s.Pair.Id == pairTwo.Id);

        Assert.AreEqual(6, pairOneStanding.GamesFor);
        Assert.AreEqual(3, pairOneStanding.GamesAgainst);
        Assert.AreEqual(3, pairOneStanding.GameDifference);

        Assert.AreEqual(3, pairTwoStanding.GamesFor);
        Assert.AreEqual(6, pairTwoStanding.GamesAgainst);
        Assert.AreEqual(-3, pairTwoStanding.GameDifference);
    }

    [TestMethod]
    public void CalculateStandings_WhenMultipleMatchesExist_OrdersByPointsAndGameDifference()
    {
        var group = new Group("Group A");

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");
        var pairThree = CreatePair("Lucas", "Mati");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.AddPair(pairThree);
        group.GenerateMatches();

        group.Matches[0].RegisterResult(new MatchResult(6, 3)); 
        group.Matches[1].RegisterResult(new MatchResult(4, 6)); 
        group.Matches[2].RegisterResult(new MatchResult(6, 2)); 

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        Assert.AreEqual(pairOne.Id, standings[0].Pair.Id);
        Assert.AreEqual(pairTwo.Id, standings[1].Pair.Id);
        Assert.AreEqual(pairThree.Id, standings[2].Pair.Id);
    }

    [TestMethod]
    public void CalculateStandings_WhenMatchHasNoResult_IgnoresMatch()
    {
        var group = new Group("Group A");

        var pairOne = CreatePair("Juan", "Pedro");
        var pairTwo = CreatePair("Nico", "Santi");

        group.AddPair(pairOne);
        group.AddPair(pairTwo);
        group.GenerateMatches();

        var service = new StandingService();

        var standings = service.CalculateStandings(group);

        Assert.IsTrue(standings.All(s => s.Played == 0));
        Assert.IsTrue(standings.All(s => s.Points == 0));
        Assert.IsTrue(standings.All(s => s.GamesFor == 0));
        Assert.IsTrue(standings.All(s => s.GamesAgainst == 0));
    }

    private static Pair CreatePair(string playerOneName, string playerTwoName)
    {
        var playerOne = new Player(playerOneName);
        var playerTwo = new Player(playerTwoName);

        return new Pair(playerOne, playerTwo);
    }
}