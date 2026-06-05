using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class OrganizerTests
{
    [TestMethod]
    public void Constructor_WithValidData_ShouldCreateOrganizer()
    {
        Organizer organizer = new Organizer(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        Assert.AreNotEqual(Guid.Empty, organizer.Id);
        Assert.AreEqual("Juan Perez", organizer.RealName);
        Assert.AreEqual("juan@mail.com", organizer.Email);
        Assert.AreEqual("Club Carrasco", organizer.ClubName);
        Assert.AreEqual("Montevideo", organizer.City);
        Assert.AreEqual("099123456", organizer.Phone);
        Assert.IsTrue(organizer.HasCompleteProfile);
    }

    [TestMethod]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Organizer(
                "",
                "juan@mail.com",
                "Club Carrasco",
                "Montevideo",
                "099123456"));
    }

    [TestMethod]
    public void Constructor_WithInvalidEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Organizer(
                "Juan Perez",
                "juanmail.com",
                "Club Carrasco",
                "Montevideo",
                "099123456"));
    }

    [TestMethod]
    public void Constructor_WithEmptyClubName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Organizer(
                "Juan Perez",
                "juan@mail.com",
                "",
                "Montevideo",
                "099123456"));
    }

    [TestMethod]
    public void Constructor_WithExtraSpaces_ShouldTrimData()
    {
        Organizer organizer = new Organizer(
            "  Juan Perez  ",
            "  JUAN@mail.com  ",
            "  Club Carrasco  ",
            "  Montevideo  ",
            "  099123456  ");

        Assert.AreEqual("Juan Perez", organizer.RealName);
        Assert.AreEqual("juan@mail.com", organizer.Email);
        Assert.AreEqual("Club Carrasco", organizer.ClubName);
        Assert.AreEqual("Montevideo", organizer.City);
        Assert.AreEqual("099123456", organizer.Phone);
    }

    [TestMethod]
    public void UpdateProfile_WithValidData_ShouldUpdateOrganizer()
    {
        Organizer organizer = new Organizer(
            "Juan Perez",
            "juan@mail.com",
            "Club Carrasco",
            "Montevideo",
            "099123456");

        organizer.UpdateProfile(
            "Ana Garcia",
            "ana@mail.com",
            "Club Prado",
            "Canelones",
            "098654321");

        Assert.AreEqual("Ana Garcia", organizer.RealName);
        Assert.AreEqual("ana@mail.com", organizer.Email);
        Assert.AreEqual("Club Prado", organizer.ClubName);
        Assert.AreEqual("Canelones", organizer.City);
        Assert.AreEqual("098654321", organizer.Phone);
    }
}