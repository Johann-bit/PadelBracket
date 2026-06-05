using PadelBracket.Domain.Enums;
using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class PlayerAccountServiceTests
{
    [TestMethod]
    public void Register_WithValidData_ShouldCreatePlayerAndLogIn()
    {
        PlayerAccountService service = CreateService();

        var player = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.IsTrue(service.IsLoggedIn);
        Assert.AreEqual(player.Id, service.CurrentPlayer!.Id);
        Assert.AreEqual("Juan Perez", service.CurrentPlayer.Name);
        Assert.AreEqual("juan@mail.com", service.CurrentPlayer.Email);
    }

    [TestMethod]
    public void Register_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        PlayerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.ThrowsException<ArgumentException>(() =>
            service.Register(
                "Ana Garcia",
                "JUAN@mail.com",
                "Drive#2026",
                "Drive#2026",
                DominantHand.Left,
                PreferredSide.Backhand,
                5));
    }

    [TestMethod]
    public void Login_WithValidCredentials_ShouldSetCurrentPlayer()
    {
        PlayerAccountService service = CreateService();

        var player = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        service.Logout();

        var loggedPlayer = service.Login("JUAN@mail.com", "Raqueta#2026");

        Assert.IsTrue(service.IsLoggedIn);
        Assert.AreEqual(player.Id, loggedPlayer.Id);
        Assert.AreEqual(player.Id, service.CurrentPlayer!.Id);
    }

    [TestMethod]
    public void Login_WithInvalidPassword_ShouldThrowInvalidOperationException()
    {
        PlayerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        service.Logout();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Login("juan@mail.com", "Incorrecta#2026"));
    }

    [TestMethod]
    public void UpdateCurrentPlayerPersonalData_WithValidEmail_ShouldUpdateLoginEmail()
    {
        PlayerAccountService service = CreateService();

        var player = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        service.UpdateCurrentPlayerPersonalData(
            "Juan Perez",
            "juan.nuevo@mail.com");

        Assert.AreEqual("juan.nuevo@mail.com", service.CurrentPlayer!.Email);

        service.Logout();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Login("juan@mail.com", "Raqueta#2026"));

        var loggedPlayer = service.Login("juan.nuevo@mail.com", "Raqueta#2026");

        Assert.AreEqual(player.Id, loggedPlayer.Id);
    }

    [TestMethod]
    public void UpdateCurrentPlayerPersonalData_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        PlayerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        service.Register(
            "Ana Garcia",
            "ana@mail.com",
            "Drive#2026",
            "Drive#2026",
            DominantHand.Left,
            PreferredSide.Backhand,
            5);

        service.Login("juan@mail.com", "Raqueta#2026");

        Assert.ThrowsException<ArgumentException>(() =>
            service.UpdateCurrentPlayerPersonalData(
                "Juan Perez",
                "ana@mail.com"));
    }

    [TestMethod]
    public void ChangeCurrentPlayerPassword_WithValidData_ShouldUpdatePassword()
    {
        PlayerAccountService service = CreateService();

        var player = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        service.ChangeCurrentPlayerPassword(
            "Raqueta#2026",
            "Nueva#2026",
            "Nueva#2026");

        service.Logout();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Login("juan@mail.com", "Raqueta#2026"));

        var loggedPlayer = service.Login("juan@mail.com", "Nueva#2026");

        Assert.AreEqual(player.Id, loggedPlayer.Id);
    }

    [TestMethod]
    public void ChangeCurrentPlayerPassword_WithWrongCurrentPassword_ShouldThrowInvalidOperationException()
    {
        PlayerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.ChangeCurrentPlayerPassword(
                "Incorrecta#2026",
                "Nueva#2026",
                "Nueva#2026"));
    }

    [TestMethod]
    public void ChangeCurrentPlayerPassword_WithWeakNewPassword_ShouldThrowArgumentException()
    {
        PlayerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Raqueta#2026",
            "Raqueta#2026",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Assert.ThrowsException<ArgumentException>(() =>
            service.ChangeCurrentPlayerPassword(
                "Raqueta#2026",
                "nueva",
                "nueva"));
    }

    private static PlayerAccountService CreateService()
    {
        var playerRepository = new InMemoryPlayerRepository();
        var playerService = new PlayerService(playerRepository);

        return new PlayerAccountService(playerService);
    }
}