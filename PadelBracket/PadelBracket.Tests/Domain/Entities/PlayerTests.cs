using PadelBracket.Domain.Entities;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void Constructor_WithValidName_ShouldCreatePlayer()
    {
        string name = "Johann";

        Player player = new Player(name);

        Assert.AreNotEqual(Guid.Empty, player.Id);
        Assert.AreEqual("Johann", player.Name);
    }

    [TestMethod]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        string? name = null;

        Assert.ThrowsException<ArgumentException>(() => new Player(name!));
    }

    [TestMethod]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        string name = "";

        Assert.ThrowsException<ArgumentException>(() => new Player(name));
    }

    [TestMethod]
    public void Constructor_WithWhiteSpaceName_ShouldThrowArgumentException()
    {
        string name = "   ";

        Assert.ThrowsException<ArgumentException>(() => new Player(name));
    }

    [TestMethod]
    public void Constructor_WithNameContainingExtraSpaces_ShouldTrimName()
    {
        string name = "  Johann  ";

        Player player = new Player(name);

        Assert.AreEqual("Johann", player.Name);
    }

    [TestMethod]
    public void Rename_WithValidName_ShouldChangePlayerName()
    {
        Player player = new Player("Johann");

        player.Rename("Franco");

        Assert.AreEqual("Franco", player.Name);
    }

    [TestMethod]
    public void Rename_WithNameContainingExtraSpaces_ShouldTrimName()
    {
        Player player = new Player("Johann");

        player.Rename("  Franco  ");

        Assert.AreEqual("Franco", player.Name);
    }

    [TestMethod]
    public void Rename_WithNullName_ShouldThrowArgumentException()
    {
        Player player = new Player("Johann");
        string? newName = null;

        Assert.ThrowsException<ArgumentException>(() => player.Rename(newName!));
    }

    [TestMethod]
    public void Rename_WithEmptyName_ShouldThrowArgumentException()
    {
        Player player = new Player("Johann");

        Assert.ThrowsException<ArgumentException>(() => player.Rename(""));
    }

    [TestMethod]
    public void Rename_WithWhiteSpaceName_ShouldThrowArgumentException()
    {
        Player player = new Player("Johann");

        Assert.ThrowsException<ArgumentException>(() => player.Rename("   "));
    }
}