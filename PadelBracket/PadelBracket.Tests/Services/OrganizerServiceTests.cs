using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class OrganizerServiceTests
{
    [TestMethod]
    public void Add_WithValidData_ShouldCreateOrganizer()
    {
        OrganizerService service = CreateService();

        var organizer = service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        Assert.AreNotEqual(Guid.Empty, organizer.Id);
        Assert.AreEqual(1, service.GetAll().Count);
        Assert.AreEqual("juan@mail.com", service.GetAll()[0].Email);
    }

    [TestMethod]
    public void Add_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        OrganizerService service = CreateService();

        service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        Assert.ThrowsException<ArgumentException>(() =>
            service.Add(
                "Ana Garcia",
                "JUAN@mail.com",
                "Club Prado",
                "Montevideo",
                "098654321"));
    }

    [TestMethod]
    public void GetDtoById_WithExistingOrganizer_ShouldReturnDto()
    {
        OrganizerService service = CreateService();

        var organizer = service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        var dto = service.GetDtoById(organizer.Id);

        Assert.IsNotNull(dto);
        Assert.AreEqual(organizer.Id, dto!.Id);
        Assert.AreEqual("Juan Perez", dto.RealName);
        Assert.AreEqual("Club Carrasco", dto.ClubName);
    }

    [TestMethod]
    public void UpdateProfile_WithValidData_ShouldUpdateOrganizer()
    {
        OrganizerService service = CreateService();

        var organizer = service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.UpdateProfile(
            organizer.Id,
            "Ana Garcia",
            "ana@mail.com",
            "Club Prado",
            "Canelones",
            "098654321");

        var updatedOrganizer = service.GetById(organizer.Id);

        Assert.IsNotNull(updatedOrganizer);
        Assert.AreEqual("Ana Garcia", updatedOrganizer!.RealName);
        Assert.AreEqual("ana@mail.com", updatedOrganizer.Email);
        Assert.AreEqual("Club Prado", updatedOrganizer.ClubName);
    }

    [TestMethod]
    public void UpdateProfile_WithDuplicateEmail_ShouldThrowArgumentException()
    {
        OrganizerService service = CreateService();

        var organizer = service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.Add(
            "Ana Garcia",
            "ana@mail.com",
            "Club Prado",
            "Montevideo",
            "098654321");

        Assert.ThrowsException<ArgumentException>(() =>
            service.UpdateProfile(
                organizer.Id,
                "Juan Perez",
                "ana@mail.com",
                "Club Carrasco",
                "Montevideo",
                "099123456"));
    }

    [TestMethod]
    public void Delete_WithExistingOrganizer_ShouldRemoveOrganizer()
    {
        OrganizerService service = CreateService();

        var organizer = service.Add(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        service.Delete(organizer.Id);

        Assert.AreEqual(0, service.GetAll().Count);
    }

    private static OrganizerService CreateService()
    {
        return new OrganizerService(new InMemoryOrganizerRepository());
    }
}