using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class TournamentServiceTests
{
    private static TournamentService CreateTournamentService()
    {
        return CreateTournamentContext().TournamentService;
    }

    private static (TournamentService TournamentService, PairService PairService) CreateTournamentContext()
    {
        var pairRepository = new InMemoryPairRepository();
        var tournamentRepository = new InMemoryTournamentRepository();
        var pairService = new PairService(pairRepository);
        var tournamentRegistrationService = new TournamentRegistrationService();

        var tournamentService = new TournamentService(
            tournamentRepository,
            pairService,
            tournamentRegistrationService);

        return (tournamentService, pairService);
    }

    [TestMethod]
    public void CreateTournament_WhenNameIsValid_AddsTournamentToList()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var tournaments = service.GetAllTournaments();

        Assert.AreEqual(1, tournaments.Count);
        Assert.AreEqual(tournament.Id, tournaments[0].Id);
        Assert.AreEqual("Torneo Apertura", tournaments[0].Name);
    }

    [TestMethod]
    public void GetTournamentById_WhenTournamentExists_ReturnsTournament()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var foundTournament = service.GetTournamentById(tournament.Id);

        Assert.IsNotNull(foundTournament);
        Assert.AreEqual(tournament.Id, foundTournament.Id);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentExists_AddsGroup()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        Assert.AreEqual(1, tournament.Groups.Count);
        Assert.AreEqual(group.Id, tournament.Groups[0].Id);
        Assert.AreEqual("Group A", tournament.Groups[0].Name);
        Assert.AreEqual(5, tournament.Groups[0].Category);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsValid_AddsGroupWithCategory()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        Assert.AreEqual(5, group.Category);
        Assert.AreEqual("5ta categoría", group.CategoryLabel);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsLowerThanOne_ThrowsArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(tournament.Id, "Group A", 0)
        );
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsGreaterThanEight_ThrowsArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(tournament.Id, "Group A", 9)
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupExists_AddsPair()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

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
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        Assert.AreEqual(3, group.Matches.Count);
    }

    [TestMethod]
    public void GenerateGroupMatches_WhenMatchesAlreadyExist_ThrowsExceptionAndKeepsExistingMatches()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        var originalMatchesCount = group.Matches.Count;
        var originalMatchIds = group.Matches.Select(match => match.Id).ToList();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateGroupMatches(tournament.Id, group.Id)
        );

        Assert.AreEqual(originalMatchesCount, group.Matches.Count);
        CollectionAssert.AreEqual(
            originalMatchIds,
            group.Matches.Select(match => match.Id).ToList()
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenMatchesAlreadyExist_ThrowsException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.AddPairToGroup(
                tournament.Id,
                group.Id,
                "Lucas",
                "Mati"
            )
        );

        Assert.AreEqual(2, group.Pairs.Count);
    }

    [TestMethod]
    public void RegisterMatchResult_WhenMatchExists_SavesResult()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        var match = group.Matches.First();

        service.RegisterMatchResult(
            tournament.Id,
            group.Id,
            match.Id,
            new List<MatchSet>
            {
                new MatchSet(6, 3),
                new MatchSet(6, 4)
            }
        );

        Assert.IsTrue(match.HasResult);
        Assert.IsNotNull(match.Result);
        Assert.AreEqual(12, match.Result.PairOneGames);
        Assert.AreEqual(7, match.Result.PairTwoGames);
        Assert.AreEqual(2, match.Result.PairOneSetsWon);
        Assert.AreEqual(0, match.Result.PairTwoSetsWon);
    }

    [TestMethod]
    public void CancelRegistration_WhenRegistrationExists_ShouldCancelRegistration()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.CancelRegistration(tournament.Id, registration.Id);

        Assert.AreEqual(RegistrationStatus.Cancelled, registration.Status);
        Assert.AreEqual(PaymentStatus.Cancelled, registration.PaymentStatus);
    }

    [TestMethod]
    public void CancelRegistration_WhenRegistrationDoesNotExist_ShouldThrowArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.CancelRegistration(tournament.Id, Guid.NewGuid()));
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentDoesNotExist_ThrowsException()
    {
        var service = CreateTournamentService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(Guid.NewGuid(), "Group A", 5)
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupDoesNotExist_ThrowsException()
    {
        var service = CreateTournamentService();

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

    private static Pair CreateCompletePair(
        PairService pairService,
        string playerOneName,
        string playerTwoName,
        int category)
    {
        Player playerOne = new Player(
            playerOneName,
            $"{playerOneName.Replace(" ", "").ToLower()}@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            category);

        Player playerTwo = new Player(
            playerTwoName,
            $"{playerTwoName.Replace(" ", "").ToLower()}@mail.com",
            DominantHand.Right,
            PreferredSide.Backhand,
            category);

        return pairService.Add(playerOne, playerTwo);
    }
}