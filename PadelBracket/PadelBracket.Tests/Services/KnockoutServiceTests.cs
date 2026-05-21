using PadelBracket.Domain.Entities;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class KnockoutServiceTests
{
    [TestMethod]
    public void GenerateSemifinals_WhenTournamentIsNull_ThrowsArgumentNullException()
    {
        var service = CreateService();

        Assert.ThrowsException<ArgumentNullException>(() =>
            service.GenerateSemifinals(null!, 5)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenCategoryIsLowerThanOne_ThrowsArgumentException()
    {
        var tournament = new Tournament("Torneo Apertura");
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.GenerateSemifinals(tournament, 0)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenCategoryIsGreaterThanEight_ThrowsArgumentException()
    {
        var tournament = new Tournament("Torneo Apertura");
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.GenerateSemifinals(tournament, 9)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenCategoryHasLessThanTwoGroups_ThrowsInvalidOperationException()
    {
        var tournament = new Tournament("Torneo Apertura");

        var groupA = CreateCompletedGroup("Grupo A", 5);
        tournament.AddGroup(groupA);

        var service = CreateService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateSemifinals(tournament, 5)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenCategoryHasMoreThanTwoGroups_ThrowsInvalidOperationException()
    {
        var tournament = new Tournament("Torneo Apertura");

        tournament.AddGroup(CreateCompletedGroup("Grupo A", 5));
        tournament.AddGroup(CreateCompletedGroup("Grupo B", 5));
        tournament.AddGroup(CreateCompletedGroup("Grupo C", 5));

        var service = CreateService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateSemifinals(tournament, 5)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenGroupHasNoGeneratedMatches_ThrowsInvalidOperationException()
    {
        var tournament = new Tournament("Torneo Apertura");

        var groupA = CreateGroupWithPairs("Grupo A", 5);
        var groupB = CreateCompletedGroup("Grupo B", 5);

        tournament.AddGroup(groupA);
        tournament.AddGroup(groupB);

        var service = CreateService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateSemifinals(tournament, 5)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenGroupHasPendingMatches_ThrowsInvalidOperationException()
    {
        var tournament = new Tournament("Torneo Apertura");

        var groupA = CreateGroupWithPairs("Grupo A", 5);
        groupA.GenerateMatches();

        var groupB = CreateCompletedGroup("Grupo B", 5);

        tournament.AddGroup(groupA);
        tournament.AddGroup(groupB);

        var service = CreateService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateSemifinals(tournament, 5)
        );
    }

    [TestMethod]
    public void GenerateSemifinals_WhenTwoGroupsAreCompleted_ReturnsTwoSemifinals()
    {
        var tournament = new Tournament("Torneo Apertura");

        tournament.AddGroup(CreateCompletedGroup("Grupo A", 5));
        tournament.AddGroup(CreateCompletedGroup("Grupo B", 5));

        var service = CreateService();

        var semifinals = service.GenerateSemifinals(tournament, 5);

        Assert.AreEqual(2, semifinals.Count);
        Assert.AreEqual("Semifinal 1", semifinals[0].RoundName);
        Assert.AreEqual("Semifinal 2", semifinals[1].RoundName);
    }

    [TestMethod]
    public void GenerateSemifinals_WhenTwoGroupsAreCompleted_CrossesFirstGroupWinnerAgainstSecondGroupRunnerUp()
    {
        var tournament = new Tournament("Torneo Apertura");

        var groupA = CreateCompletedGroup("Grupo A", 5);
        var groupB = CreateCompletedGroup("Grupo B", 5);

        tournament.AddGroup(groupA);
        tournament.AddGroup(groupB);

        var standingService = new StandingService();
        var service = new KnockoutService(standingService);

        var groupAStandings = standingService.CalculateStandings(groupA);
        var groupBStandings = standingService.CalculateStandings(groupB);

        var semifinals = service.GenerateSemifinals(tournament, 5);

        Assert.AreEqual(groupAStandings[0].Pair.Id, semifinals[0].PairOne!.Id);
        Assert.AreEqual(groupBStandings[1].Pair.Id, semifinals[0].PairTwo!.Id);
    }

    [TestMethod]
    public void GenerateSemifinals_WhenTwoGroupsAreCompleted_CrossesSecondGroupWinnerAgainstFirstGroupRunnerUp()
    {
        var tournament = new Tournament("Torneo Apertura");

        var groupA = CreateCompletedGroup("Grupo A", 5);
        var groupB = CreateCompletedGroup("Grupo B", 5);

        tournament.AddGroup(groupA);
        tournament.AddGroup(groupB);

        var standingService = new StandingService();
        var service = new KnockoutService(standingService);

        var groupAStandings = standingService.CalculateStandings(groupA);
        var groupBStandings = standingService.CalculateStandings(groupB);

        var semifinals = service.GenerateSemifinals(tournament, 5);

        Assert.AreEqual(groupBStandings[0].Pair.Id, semifinals[1].PairOne!.Id);
        Assert.AreEqual(groupAStandings[1].Pair.Id, semifinals[1].PairTwo!.Id);
    }

    [TestMethod]
    public void GenerateSemifinals_WhenOtherCategoriesExist_IgnoresOtherCategories()
    {
        var tournament = new Tournament("Torneo Apertura");

        tournament.AddGroup(CreateCompletedGroup("Grupo A", 5));
        tournament.AddGroup(CreateCompletedGroup("Grupo B", 5));
        tournament.AddGroup(CreateCompletedGroup("Grupo A", 6));

        var service = CreateService();

        var semifinals = service.GenerateSemifinals(tournament, 5);

        Assert.AreEqual(2, semifinals.Count);
        Assert.IsTrue(semifinals.All(match => match.IsReadyToPlay));
    }

    private static KnockoutService CreateService()
    {
        return new KnockoutService(new StandingService());
    }

    private static Group CreateCompletedGroup(string name, int category)
    {
        var group = CreateGroupWithPairs(name, category);

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2
        RegisterPairOneWin(group.Matches[1]); // Pair 1 beats Pair 3
        RegisterPairOneWin(group.Matches[2]); // Pair 2 beats Pair 3

        return group;
    }

    private static Group CreateGroupWithPairs(string name, int category)
    {
        var group = new Group(name, category);

        group.AddPair(CreatePair($"{name} Jugador 1A", $"{name} Jugador 1B"));
        group.AddPair(CreatePair($"{name} Jugador 2A", $"{name} Jugador 2B"));
        group.AddPair(CreatePair($"{name} Jugador 3A", $"{name} Jugador 3B"));

        return group;
    }

    private static Pair CreatePair(string playerOneName, string playerTwoName)
    {
        var playerOne = new Player(playerOneName);
        var playerTwo = new Player(playerTwoName);

        return new Pair(playerOne, playerTwo);
    }

    private static void RegisterPairOneWin(Match match)
    {
        match.RegisterResult(new MatchResult(new List<MatchSet>
        {
            new MatchSet(6, 3),
            new MatchSet(6, 4)
        }));
    }
}