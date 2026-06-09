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

        AddRequiredThirdPair(context, tournament.Id, group, 6, "Ranking");

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
        AddRequiredThirdPair(context, tournament.Id, sixthCategoryGroup, 6, "Sexta");
        AddRequiredThirdPair(context, tournament.Id, fifthCategoryGroup, 5, "Quinta");

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

    [TestMethod]
    public void GetPlayerRanking_WhenMatchesHaveResults_ShouldGivePointsToBothPlayersOfWinningPair()
    {
        var context = CreateContext();

        var tournament = context.TournamentService.CreateTournament("Torneo Apertura");

        var group = context.TournamentService.AddGroupToTournament(
            tournament.Id,
            "Grupo A",
            6);

        var winningPair = CreateCompletePair(
            context.PairService,
            "Juan Perez",
            "Pedro Gomez",
            6);

        var losingPair = CreateCompletePair(
            context.PairService,
            "Nico Silva",
            "Santi Lopez",
            6);

        AddPairToGroup(context, tournament.Id, group.Id, winningPair.Id);
        AddPairToGroup(context, tournament.Id, group.Id, losingPair.Id);
        AddRequiredThirdPair(context, tournament.Id, group, 6, "Players");

        RegisterFirstMatchResult(context, tournament.Id, group);

        var ranking = context.RankingService.GetPlayerRanking();

        Assert.AreEqual(4, ranking.Count);

        var playerOneRanking = ranking.First(item => item.PlayerId == winningPair.PlayerOne.Id);
        var playerTwoRanking = ranking.First(item => item.PlayerId == winningPair.PlayerTwo.Id);
        var losingPlayerOneRanking = ranking.First(item => item.PlayerId == losingPair.PlayerOne.Id);
        var losingPlayerTwoRanking = ranking.First(item => item.PlayerId == losingPair.PlayerTwo.Id);

        Assert.AreEqual(3, playerOneRanking.Points);
        Assert.AreEqual(3, playerTwoRanking.Points);
        Assert.AreEqual(1, playerOneRanking.Wins);
        Assert.AreEqual(1, playerTwoRanking.Wins);

        Assert.AreEqual(0, losingPlayerOneRanking.Points);
        Assert.AreEqual(0, losingPlayerTwoRanking.Points);
        Assert.AreEqual(1, losingPlayerOneRanking.Losses);
        Assert.AreEqual(1, losingPlayerTwoRanking.Losses);
    }

    [TestMethod]
    public void GetPlayerRankingByCategory_ShouldReturnOnlyPlayersFromSelectedCategory()
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
        AddRequiredThirdPair(context, tournament.Id, sixthCategoryGroup, 6, "Sexta");
        AddRequiredThirdPair(context, tournament.Id, fifthCategoryGroup, 5, "Quinta");

        RegisterFirstMatchResult(context, tournament.Id, sixthCategoryGroup);
        RegisterFirstMatchResult(context, tournament.Id, fifthCategoryGroup);

        var ranking = context.RankingService.GetPlayerRankingByCategory(6);

        Assert.AreEqual(4, ranking.Count);
        Assert.IsTrue(ranking.All(item => item.Category == 6));
        Assert.IsTrue(ranking.Any(item => item.PlayerId == sixthCategoryPairOne.PlayerOne.Id));
        Assert.IsTrue(ranking.Any(item => item.PlayerId == sixthCategoryPairOne.PlayerTwo.Id));
        Assert.IsTrue(ranking.Any(item => item.PlayerId == sixthCategoryPairTwo.PlayerOne.Id));
        Assert.IsTrue(ranking.Any(item => item.PlayerId == sixthCategoryPairTwo.PlayerTwo.Id));
        Assert.IsFalse(ranking.Any(item => item.PlayerId == fifthCategoryPairOne.PlayerOne.Id));
        Assert.IsFalse(ranking.Any(item => item.PlayerId == fifthCategoryPairTwo.PlayerOne.Id));
    }

    [TestMethod]
    public void GetPlayerRankingByCategory_WhenCategoryIsInvalid_ShouldThrowArgumentException()
    {
        var context = CreateContext();

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetPlayerRankingByCategory(0));

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetPlayerRankingByCategory(9));
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

    [TestMethod]
    public void GetPlayerRankingSummary_WhenPlayerIsRanked_ShouldReturnPositionAndStats()
    {
        var context = CreateContext();

        var tournament = context.TournamentService.CreateTournament("Torneo Apertura");

        var group = context.TournamentService.AddGroupToTournament(
            tournament.Id,
            "Grupo A",
            6);

        var winningPair = CreateCompletePair(
            context.PairService,
            "Juan Perez",
            "Pedro Gomez",
            6);

        var losingPair = CreateCompletePair(
            context.PairService,
            "Nico Silva",
            "Santi Lopez",
            6);

        AddPairToGroup(context, tournament.Id, group.Id, winningPair.Id);
        AddPairToGroup(context, tournament.Id, group.Id, losingPair.Id);
        AddRequiredThirdPair(context, tournament.Id, group, 6, "Summary");

        RegisterFirstMatchResult(context, tournament.Id, group);

        var summary = context.RankingService.GetPlayerRankingSummary(
            winningPair.PlayerOne.Id,
            6);

        Assert.IsTrue(summary.IsRanked);
        Assert.AreEqual(winningPair.PlayerOne.Id, summary.PlayerId);
        Assert.AreEqual(6, summary.Category);
        Assert.AreEqual(1, summary.Position);
        Assert.IsNotNull(summary.Ranking);
        Assert.AreEqual(3, summary.Ranking.Points);
        Assert.AreEqual(1, summary.Ranking.Wins);
        Assert.AreEqual(0, summary.Ranking.Losses);
    }

    [TestMethod]
    public void GetPlayerRankingSummary_WhenPlayerHasNoResults_ShouldReturnUnrankedSummary()
    {
        var context = CreateContext();
        Guid playerId = Guid.NewGuid();

        var summary = context.RankingService.GetPlayerRankingSummary(playerId, 6);

        Assert.IsFalse(summary.IsRanked);
        Assert.AreEqual(playerId, summary.PlayerId);
        Assert.AreEqual(6, summary.Category);
        Assert.IsNull(summary.Position);
        Assert.IsNull(summary.Ranking);
    }

    [TestMethod]
    public void GetPlayerRankingSummary_WhenPlayerIdIsEmpty_ShouldThrowArgumentException()
    {
        var context = CreateContext();

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetPlayerRankingSummary(Guid.Empty, 6));
    }

    [TestMethod]
    public void GetPlayerRankingSummary_WhenCategoryIsInvalid_ShouldThrowArgumentException()
    {
        var context = CreateContext();

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetPlayerRankingSummary(Guid.NewGuid(), 0));

        Assert.ThrowsException<ArgumentException>(() =>
            context.RankingService.GetPlayerRankingSummary(Guid.NewGuid(), 9));
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

    private static void AddRequiredThirdPair(
        TestContext context,
        Guid tournamentId,
        Group group,
        int category,
        string suffix)
    {
        var thirdPair = CreateCompletePair(
            context.PairService,
            $"Tercer Jugador {suffix}",
            $"Cuarto Jugador {suffix}",
            category);

        AddPairToGroup(context, tournamentId, group.Id, thirdPair.Id);
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
