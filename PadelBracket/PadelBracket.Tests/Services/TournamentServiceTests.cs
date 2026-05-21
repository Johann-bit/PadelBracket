using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class TournamentServiceTests
{
    [TestMethod]
    public void CreateTournament_WhenNameIsValid_AddsTournamentToList()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var tournaments = service.GetAllTournaments();

        Assert.AreEqual(1, tournaments.Count);
        Assert.AreEqual(tournament.Id, tournaments[0].Id);
        Assert.AreEqual("Torneo Apertura", tournaments[0].Name);
    }

    [TestMethod]
    public void GetTournamentById_WhenTournamentExists_ReturnsTournament()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var foundTournament = service.GetTournamentById(tournament.Id);

        Assert.IsNotNull(foundTournament);
        Assert.AreEqual(tournament.Id, foundTournament.Id);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentExists_AddsGroup()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var group = service.AddGroupToTournament(tournament.Id, "Group A");

        Assert.AreEqual(1, tournament.Groups.Count);
        Assert.AreEqual(group.Id, tournament.Groups[0].Id);
        Assert.AreEqual("Group A", tournament.Groups[0].Name);
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupExists_AddsPair()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A");

        var pair = service.AddPairToGroup(
            tournament.Id,
            group.Id,
            "Juan",
            "Pedro"
        );

        Assert.AreEqual(1, group.Pairs.Count);
        Assert.AreEqual(pair.Id, group.Pairs[0].Id);
        Assert.AreEqual("Juan / Pedro", group.Pairs[0].DisplayName);
    }

    [TestMethod]
    public void GenerateGroupMatches_WhenGroupHasThreePairs_GeneratesThreeMatches()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A");

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        Assert.AreEqual(3, group.Matches.Count);
    }

    [TestMethod]
    public void RegisterMatchResult_WhenMatchExists_SavesResult()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A");

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        var match = group.Matches.First();

        service.RegisterMatchResult(
            tournament.Id,
            group.Id,
            match.Id,
            6,
            3
        );

        Assert.IsTrue(match.HasResult);
        Assert.IsNotNull(match.Result);
        Assert.AreEqual(6, match.Result.PairOneGames);
        Assert.AreEqual(3, match.Result.PairTwoGames);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentDoesNotExist_ThrowsException()
    {
        var service = new TournamentService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(Guid.NewGuid(), "Group A")
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupDoesNotExist_ThrowsException()
    {
        var service = new TournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddPairToGroup(
                tournament.Id,
                Guid.NewGuid(),
                "Juan",
                "Pedro"
            )
        );
    }
}