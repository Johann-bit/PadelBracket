using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class PairServiceTests
{
    [TestMethod]
    public void Add_WithValidPlayers_ShouldCreatePair()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);

        Pair pair = service.Add(playerOne, playerTwo);

        Assert.AreNotEqual(Guid.Empty, pair.Id);
        Assert.AreEqual(playerOne.Id, pair.PlayerOne.Id);
        Assert.AreEqual(playerTwo.Id, pair.PlayerTwo.Id);
        Assert.AreEqual(1, service.GetAll().Count);
    }

    [TestMethod]
    public void Add_WhenPlayerOneAlreadyHasPair_ShouldThrowInvalidOperationException()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);
        Player playerThree = CreateCompletePlayer("Bruno Lopez", "bruno@mail.com", 6);

        service.Add(playerOne, playerTwo);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Add(playerOne, playerThree));
    }

    [TestMethod]
    public void Add_WhenPlayerTwoAlreadyHasPair_ShouldThrowInvalidOperationException()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);
        Player playerThree = CreateCompletePlayer("Bruno Lopez", "bruno@mail.com", 6);

        service.Add(playerOne, playerTwo);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Add(playerThree, playerTwo));
    }

    [TestMethod]
    public void Add_WithSamePairTwice_ShouldThrowArgumentException()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);

        service.Add(playerOne, playerTwo);

        Assert.ThrowsException<ArgumentException>(() =>
            service.Add(playerOne, playerTwo));
    }

    [TestMethod]
    public void PlayerHasPair_WhenPlayerBelongsToPair_ShouldReturnTrue()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);

        service.Add(playerOne, playerTwo);

        Assert.IsTrue(service.PlayerHasPair(playerOne.Id));
        Assert.IsTrue(service.PlayerHasPair(playerTwo.Id));
    }

    [TestMethod]
    public void PlayerHasPair_WhenPlayerDoesNotBelongToPair_ShouldReturnFalse()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);

        Assert.IsFalse(service.PlayerHasPair(playerOne.Id));
    }

    [TestMethod]
    public void GetDtosByPlayerId_WhenPlayerHasPair_ShouldReturnPlayerPair()
    {
        PairService service = CreateService();
        Player playerOne = CreateCompletePlayer("Juan Perez", "juan@mail.com", 6);
        Player playerTwo = CreateCompletePlayer("Ana Garcia", "ana@mail.com", 6);

        Pair pair = service.Add(playerOne, playerTwo);

        var playerPairs = service.GetDtosByPlayerId(playerOne.Id);

        Assert.AreEqual(1, playerPairs.Count);
        Assert.AreEqual(pair.Id, playerPairs[0].Id);
    }

    private static PairService CreateService()
    {
        return new PairService(new InMemoryPairRepository());
    }

    private static Player CreateCompletePlayer(string name, string email, int category)
    {
        return new Player(
            name,
            email,
            DominantHand.Right,
            PreferredSide.Drive,
            category);
    }
}