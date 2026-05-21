using PadelBracket.Domain.Entities;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class QualificationServiceTests
{
    [TestMethod]
    public void CalculateQualifications_WhenGroupIsNull_ThrowsArgumentNullException()
    {
        var service = new QualificationService();

        Assert.ThrowsException<ArgumentNullException>(() =>
            service.CalculateQualifications(null!, 2)
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenQualifiedPairsCountIsZero_ThrowsArgumentException()
    {
        var group = CreateGroupWithPairs(3);
        var service = new QualificationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.CalculateQualifications(group, 0)
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenQualifiedPairsCountIsNegative_ThrowsArgumentException()
    {
        var group = CreateGroupWithPairs(3);
        var service = new QualificationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.CalculateQualifications(group, -1)
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenQualifiedPairsCountIsEqualToPairsCount_ThrowsArgumentException()
    {
        var group = CreateGroupWithPairs(2);
        var service = new QualificationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.CalculateQualifications(group, 2)
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenQualifiedPairsCountIsGreaterThanPairsCount_ThrowsArgumentException()
    {
        var group = CreateGroupWithPairs(2);
        var service = new QualificationService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.CalculateQualifications(group, 3)
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenGroupHasPairs_ReturnsOneQualificationPerPair()
    {
        var group = CreateGroupWithPairs(4);
        group.GenerateMatches();

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        Assert.AreEqual(4, qualifications.Count);

        foreach (var pair in group.Pairs)
        {
            Assert.IsTrue(qualifications.Any(q => q.Pair.Id == pair.Id));
        }
    }

    [TestMethod]
    public void CalculateQualifications_WhenMatchesAreCompleted_OrdersQualificationsByCurrentPosition()
    {
        var group = CreateGroupWithPairs(3);

        var pairOne = group.Pairs[0];
        var pairTwo = group.Pairs[1];
        var pairThree = group.Pairs[2];

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2
        RegisterPairOneWin(group.Matches[1]); // Pair 1 beats Pair 3
        RegisterPairOneWin(group.Matches[2]); // Pair 2 beats Pair 3

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        Assert.AreEqual(pairOne.Id, qualifications[0].Pair.Id);
        Assert.AreEqual(1, qualifications[0].CurrentPosition);

        Assert.AreEqual(pairTwo.Id, qualifications[1].Pair.Id);
        Assert.AreEqual(2, qualifications[1].CurrentPosition);

        Assert.AreEqual(pairThree.Id, qualifications[2].Pair.Id);
        Assert.AreEqual(3, qualifications[2].CurrentPosition);
    }

    [TestMethod]
    public void CalculateQualifications_WhenPairCannotBeEliminated_ReturnsMathematicallyQualified()
    {
        var group = CreateGroupWithPairs(3);

        var pairOne = group.Pairs[0];

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2
        RegisterPairOneWin(group.Matches[1]); // Pair 1 beats Pair 3
        RegisterPairOneWin(group.Matches[2]); // Pair 2 beats Pair 3

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        var pairOneQualification = qualifications.First(q => q.Pair.Id == pairOne.Id);

        Assert.AreEqual(
            QualificationStatus.MathematicallyQualified,
            pairOneQualification.Status
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenPairCannotQualify_ReturnsMathematicallyEliminated()
    {
        var group = CreateGroupWithPairs(3);

        var pairThree = group.Pairs[2];

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2
        RegisterPairOneWin(group.Matches[1]); // Pair 1 beats Pair 3
        RegisterPairOneWin(group.Matches[2]); // Pair 2 beats Pair 3

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        var pairThreeQualification = qualifications.First(q => q.Pair.Id == pairThree.Id);

        Assert.AreEqual(
            QualificationStatus.MathematicallyEliminated,
            pairThreeQualification.Status
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenPairIsCurrentlyInsideZoneButCanStillBeEliminated_ReturnsInQualificationZone()
    {
        var group = CreateGroupWithPairs(4);

        var pairOne = group.Pairs[0];

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2. Other matches are pending.

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        var pairOneQualification = qualifications.First(q => q.Pair.Id == pairOne.Id);

        Assert.AreEqual(
            QualificationStatus.InQualificationZone,
            pairOneQualification.Status
        );
    }

    [TestMethod]
    public void CalculateQualifications_WhenPairIsOutsideZoneButCanStillQualify_ReturnsAlive()
    {
        var group = CreateGroupWithPairs(4);

        var pairTwo = group.Pairs[1];

        group.GenerateMatches();

        RegisterPairOneWin(group.Matches[0]); // Pair 1 beats Pair 2. Pair 2 can still qualify.

        var service = new QualificationService();

        var qualifications = service.CalculateQualifications(group, 2);

        var pairTwoQualification = qualifications.First(q => q.Pair.Id == pairTwo.Id);

        Assert.AreEqual(
            QualificationStatus.Alive,
            pairTwoQualification.Status
        );
    }

    private static Group CreateGroupWithPairs(int pairCount)
    {
        var group = new Group("Grupo A", 5);

        for (int i = 1; i <= pairCount; i++)
        {
            group.AddPair(CreatePair($"Jugador {i}A", $"Jugador {i}B"));
        }

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