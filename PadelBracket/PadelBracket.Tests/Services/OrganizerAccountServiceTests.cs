using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class OrganizerAccountServiceTests
{
    [TestMethod]
    public void Register_WithValidData_ShouldCreateOrganizerAndLogIn()
    {
        OrganizerAccountService service = CreateService();

        var organizer = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Cancha#2026",
            "Cancha#2026",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        Assert.IsTrue(service.IsLoggedIn);
        Assert.AreEqual(organizer.Id, service.CurrentOrganizer!.Id);
        Assert.AreEqual("Juan Perez", service.CurrentOrganizer.RealName);
        Assert.AreEqual("Club Carrasco", service.CurrentOrganizer.ClubName);
    }

    [TestMethod]
    public void Register_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        OrganizerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Cancha#2026",
            "Cancha#2026",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        Assert.ThrowsException<ArgumentException>(() =>
            service.Register(
                "Ana Garcia",
                "JUAN@mail.com",
                "Torneo#2026",
                "Torneo#2026",
                "Club Prado",
                "Montevideo",
                "098654321"));
    }

    [TestMethod]
    public void Login_WithValidCredentials_ShouldSetCurrentOrganizer()
    {
        OrganizerAccountService service = CreateService();

        var organizer = service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Cancha#2026",
            "Cancha#2026",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.Logout();

        var loggedOrganizer = service.Login("JUAN@mail.com", "Cancha#2026");

        Assert.IsTrue(service.IsLoggedIn);
        Assert.AreEqual(organizer.Id, loggedOrganizer.Id);
        Assert.AreEqual(organizer.Id, service.CurrentOrganizer!.Id);
    }

    [TestMethod]
    public void Login_WithInvalidPassword_ShouldThrowInvalidOperationException()
    {
        OrganizerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Cancha#2026",
            "Cancha#2026",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.Logout();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.Login("juan@mail.com", "Incorrecta#2026"));
    }

    [TestMethod]
    public void Register_WithWeakPassword_ShouldThrowArgumentException()
    {
        OrganizerAccountService service = CreateService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.Register(
                "Juan Perez",
                "juan@mail.com",
                "cancha",
                "cancha",
                "Club Carrasco",
                "Montevideo",
                "099123456"));
    }

    [TestMethod]
    public void Logout_ShouldClearCurrentOrganizer()
    {
        OrganizerAccountService service = CreateService();

        service.Register(
            "Juan Perez",
            "juan@mail.com",
            "Cancha#2026",
            "Cancha#2026",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.Logout();

        Assert.IsFalse(service.IsLoggedIn);
        Assert.IsNull(service.CurrentOrganizer);
    }

    private static OrganizerAccountService CreateService()
    {
        var organizerRepository = new InMemoryOrganizerRepository();
        var organizerService = new OrganizerService(organizerRepository);

        return new OrganizerAccountService(organizerService);
    }
}