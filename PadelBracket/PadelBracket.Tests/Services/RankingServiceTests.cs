using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class RankingServiceTests
{
    [TestMethod]
    public void GetRanking_WhenMatchesHaveResults_ShouldReturnRankingItemsWithCategory()
    {
        var context = CreateContext();

        var tournament = context.TournamentService.CreateTournament("Torneo Apertura");

        var group = context.TournamentService.AddGroupToTournament(
            tournament.Id,
            "Grupo A",
            6);

        var pairOne = CreateCompletePair(
            context.PairService,
            "Juan Perez",
            "Pedro Gomez",
            6);

        var pairTwo = CreateCompletePair(
            context.PairService,
            "Nico Silva",
            "Santi Lopez",
            6);

        context.TournamentService.AddExistingPairToGroup(
            tournament.Id,
            group.Id,
            pairOne.Id);

        context.TournamentService.AddExistingPairToGroup(
            tournament.Id,
            group.Id,
            pairTwo.Id);

        context.TournamentService.GenerateGroupMatches(tournament.Id, group.Id);

        var match = group.Matches.First();

        context.TournamentService.RegisterMatchResult(
            tournament.Id,
            group.Id,
            match.Id,
            new List<MatchSet>
            {
                new MatchSet(6, 3),
                new MatchSet(6, 4)
            });

        var ranking = context.RankingService.GetRanking();

        Assert.AreEqual(2, ranking.Count);
        Assert.IsTrue(ranking.All(item => item.Category == 6));
        Assert.AreEqual(pairOne.Id, ranking[0].PairId);
        Assert.AreEqual(3, ranking[0].Points);
        Assert.AreEqual(1, ranking[0].Wins);
        Assert.AreEqual(0, ranking[0].Losses);
        Assert.AreEqual(pairTwo.Id, ranking[1].PairId);
        Assert.AreEqual(0, ranking[1].Points);
        Assert.AreEqual(0, ranking[1].Wins);
        Assert.AreEqual(1, ranking[1].Losses);
    }

    [TestMethod]
    public void GetRankingByCategory_ShouldReturnOnlyRankingItemsFromSelectedCategory()
    {
        var context = CreateContext();

        var tournament = context.TournamentService.CreateTournament("Torneo Apertura");

        var sixthCategoryGroup = context.TournamentService.AddGroupToTournament(
            tournament.Id,
            "Grupo A",
            6);

        var fifthCategoryGroup = context.TournamentService.AddGroupToTournament(
            tournament.Id,
            "Grupo B",
            5);

        var sixthCategoryPairOne = CreateCompletePair(
            context.PairService,
            "Juan Perez",
            "Pedro Gomez",
            6);

        var sixthCategoryPairTwo = CreateCompletePair(
            context.PairService,
            "Nico Silva",
            "Santi Lopez",
            6);

        var fifthCategoryPairOne = CreateCompletePair(
            context.PairService,
            "Lucas Diaz",
            "Mateo Torres",
            5);

        var fifthCategoryPairTwo = CreateCompletePair(
            context.PairService,
            "Bruno Costa",
            "Facu Molina",
            5);

        AddPairToGroup(context, tournament.Id, sixthCategoryGroup.Id, sixthCategoryPairOne.Id);
        AddPairToGroup(context, tournament.Id, sixthCategoryGroup.Id, sixthCategoryPairTwo.Id);
        AddPairToGroup(context, tournament.Id, fifthCategoryGroup.Id, fifthCategoryPairOne.Id);
        AddPairToGroup(context, tournament.Id, fifthCategoryGroup.Id, fifthCategoryPairTwo.Id);

        RegisterFirstMatchResult(context, tournament.Id, sixthCategoryGroup);
        RegisterFirstMatchResult(context, tournament.Id, fifthCategoryGroup);

        var ranking = context.RankingService.GetRankingByCategory(6);

        Assert.AreEqual(2, ranking.Count);
        Assert.IsTrue(ranking.All(item => item.Category == 6));
        Assert.IsTrue(ranking.Any(item => item.PairId == sixthCategoryPairOne.Id));
        Assert.IsTrue(ranking.Any(item => item.PairId == sixthCategoryPairTwo.Id));
        Assert.IsFalse(ranking.Any(item => item.PairId == fifthCategoryPairOne.Id));
        Assert.IsFalse(ranking.Any(item => item.PairId == fifthCategoryPairTwo.Id));
    }

    [TestMethod]
    public void GetRankingByCategory_WhenCategoryIsInvalid_ShouldThrowArgumentException()
    {
        var context = CreateContext();

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetRankingByCategory(0));

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetRankingByCategory(9));
    }

    private static void AddPairToGroup(
        TestContext context,
        Guid tournamentId,
        Guid groupId,
        Guid pairId)
    {
        context.TournamentService.AddExistingPairToGroup(
            tournamentId,
            groupId,
            pairId);
    }

    private static void RegisterFirstMatchResult(
        TestContext context,
        Guid tournamentId,
        Group group)
    {
        context.TournamentService.GenerateGroupMatches(tournamentId, group.Id);

        var match = group.Matches.First();

        context.TournamentService.RegisterMatchResult(
            tournamentId,
            group.Id,
            match.Id,
            new List<MatchSet>
            {
                new MatchSet(6, 3),
                new MatchSet(6, 4)
            });
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

    private static TestContext CreateContext()
    {
        var pairRepository = new InMemoryPairRepository();
        var tournamentRepository = new InMemoryTournamentRepository();
        var pairService = new PairService(pairRepository);
        var tournamentRegistrationService = new TournamentRegistrationService();

        var tournamentService = new TournamentService(
            tournamentRepository,
            pairService,
            tournamentRegistrationService);

        var rankingService = new RankingService(tournamentService);

        return new TestContext(
            TournamentService: tournamentService,
            PairService: pairService,
            RankingService: rankingService);
    }

    private sealed record TestContext(
        TournamentService TournamentService,
        PairService PairService,
        RankingService RankingService);
}