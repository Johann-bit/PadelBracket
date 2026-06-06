using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Tests.Domain.Entities;

[TestClass]
public class TournamentTests
{
    [TestMethod]
    public void Constructor_WithValidName_ShouldCreateTournament()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.AreNotEqual(Guid.Empty, tournament.Id);
        Assert.IsNull(tournament.OrganizerId);
        Assert.AreEqual("Summer Cup", tournament.Name);
        Assert.AreEqual(TournamentStatus.Draft, tournament.Status);
        Assert.AreEqual("Borrador", tournament.StatusLabel);
        Assert.AreEqual(0, tournament.Groups.Count);
        Assert.AreEqual(0, tournament.TournamentCategories.Count);
        Assert.AreEqual(0, tournament.Registrations.Count);
    }

    [TestMethod]
    public void Constructor_WithOrganizerId_ShouldCreateTournamentWithOrganizer()
    {
        Guid organizerId = Guid.NewGuid();

        Tournament tournament = new Tournament("Summer Cup", organizerId);

        Assert.AreEqual(organizerId, tournament.OrganizerId);
    }

    [TestMethod]
    public void Constructor_WithEmptyOrganizerId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Tournament("Summer Cup", Guid.Empty));
    }

    [TestMethod]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Tournament(""));
    }

    [TestMethod]
    public void Constructor_WithWhiteSpaceName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Tournament("   "));
    }

    [TestMethod]
    public void Constructor_WithNameContainingExtraSpaces_ShouldTrimName()
    {
        Tournament tournament = new Tournament("  Summer Cup  ");

        Assert.AreEqual("Summer Cup", tournament.Name);
    }

    [TestMethod]
    public void Rename_WithValidName_ShouldChangeTournamentName()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.Rename("Winter Cup");

        Assert.AreEqual("Winter Cup", tournament.Name);
    }

    [TestMethod]
    public void Rename_WithEmptyName_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentException>(() => tournament.Rename(""));
    }

    [TestMethod]
    public void AddCategory_WithValidCategory_ShouldAddTournamentCategory()
    {
        Tournament tournament = new Tournament("Summer Cup");
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        tournament.AddCategory(tournamentCategory);

        Assert.AreEqual(1, tournament.TournamentCategories.Count);
        Assert.AreEqual(6, tournament.TournamentCategories[0].Category);
    }

    [TestMethod]
    public void AddCategory_WithNullCategory_ShouldThrowArgumentNullException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentNullException>(() => tournament.AddCategory(null!));
    }

    [TestMethod]
    public void AddCategory_WithDuplicatedCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Assert.ThrowsException<ArgumentException>(() =>
            tournament.AddCategory(new TournamentCategory(6, 12, 700)));
    }

    [TestMethod]
    public void AddCategory_WhenTournamentIsNotDraft_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.StartGroupStage();

        Assert.ThrowsException<InvalidOperationException>(() =>
            tournament.AddCategory(new TournamentCategory(6, 16, 800)));
    }

    [TestMethod]
    public void RemoveCategory_WithExistingCategory_ShouldRemoveCategory()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        tournament.RemoveCategory(6);

        Assert.AreEqual(0, tournament.TournamentCategories.Count);
    }

    [TestMethod]
    public void RemoveCategory_WithUnknownCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentException>(() => tournament.RemoveCategory(6));
    }

    [TestMethod]
    public void RemoveCategory_WithActiveRegistrations_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistration registration = new TournamentRegistration(tournament.Id, pair, 6);

        tournament.AddRegistration(registration);

        Assert.ThrowsException<InvalidOperationException>(() => tournament.RemoveCategory(6));
    }

    [TestMethod]
    public void RemoveCategory_WhenTournamentIsNotDraft_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddCategory(new TournamentCategory(6, 16, 800));
        tournament.StartGroupStage();

        Assert.ThrowsException<InvalidOperationException>(() => tournament.RemoveCategory(6));
    }

    [TestMethod]
    public void HasCategory_WithExistingCategory_ShouldReturnTrue()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Assert.IsTrue(tournament.HasCategory(6));
    }

    [TestMethod]
    public void HasCategory_WithUnknownCategory_ShouldReturnFalse()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.IsFalse(tournament.HasCategory(6));
    }

    [TestMethod]
    public void GetCategory_WithExistingCategory_ShouldReturnTournamentCategory()
    {
        Tournament tournament = new Tournament("Summer Cup");
        TournamentCategory tournamentCategory = new TournamentCategory(6, 16, 800);

        tournament.AddCategory(tournamentCategory);

        TournamentCategory result = tournament.GetCategory(6);

        Assert.AreEqual(tournamentCategory.Id, result.Id);
    }

    [TestMethod]
    public void GetCategory_WithUnknownCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentException>(() => tournament.GetCategory(6));
    }

    [TestMethod]
    public void AddRegistration_WithValidRegistration_ShouldAddRegistration()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistration registration = new TournamentRegistration(tournament.Id, pair, 6);

        tournament.AddRegistration(registration);

        Assert.AreEqual(1, tournament.Registrations.Count);
        Assert.AreEqual(registration.Id, tournament.Registrations[0].Id);
    }

    [TestMethod]
    public void AddRegistration_WithNullRegistration_ShouldThrowArgumentNullException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentNullException>(() => tournament.AddRegistration(null!));
    }

    [TestMethod]
    public void AddRegistration_WithDifferentTournamentId_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistration registration = new TournamentRegistration(Guid.NewGuid(), pair, 6);

        Assert.ThrowsException<ArgumentException>(() => tournament.AddRegistration(registration));
    }

    [TestMethod]
    public void AddRegistration_WithUnknownCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Pair pair = CreateVerifiedPair("Johann", "Franco", 6);
        TournamentRegistration registration = new TournamentRegistration(tournament.Id, pair, 6);

        Assert.ThrowsException<ArgumentException>(() => tournament.AddRegistration(registration));
    }

    [TestMethod]
    public void AddRegistration_WithSamePairTwice_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Player playerOne = CreateVerifiedPlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerOne, playerTwo);

        tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairOne, 6));

        Assert.ThrowsException<ArgumentException>(() =>
            tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairTwo, 6)));
    }

    [TestMethod]
    public void AddRegistration_WithSamePlayersInDifferentPairOrder_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Player playerOne = CreateVerifiedPlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerTwo, playerOne);

        tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairOne, 6));

        Assert.ThrowsException<ArgumentException>(() =>
            tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairTwo, 6)));
    }

    [TestMethod]
    public void AddRegistration_WithOnePlayerAlreadyRegisteredInAnotherPair_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));

        Player playerOne = CreateVerifiedPlayer("Johann", "johann@mail.com", 6);
        Player playerTwo = CreateVerifiedPlayer("Franco", "franco@mail.com", 6);
        Player playerThree = CreateVerifiedPlayer("Bruno", "bruno@mail.com", 6);

        Pair pairOne = new Pair(playerOne, playerTwo);
        Pair pairTwo = new Pair(playerOne, playerThree);

        tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairOne, 6));

        Assert.ThrowsException<ArgumentException>(() =>
            tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairTwo, 6)));
    }

    [TestMethod]
    public void AddRegistration_WhenCategoryHasNoAvailableSlots_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 1, 800));

        Pair pairOne = CreateVerifiedPair("Johann", "Franco", 6);
        Pair pairTwo = CreateVerifiedPair("Bruno", "Joshua", 6);

        tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairOne, 6));

        Assert.ThrowsException<InvalidOperationException>(() =>
            tournament.AddRegistration(new TournamentRegistration(tournament.Id, pairTwo, 6)));
    }

    [TestMethod]
    public void GetActiveRegistrationsByCategory_ShouldReturnOnlyActiveRegistrationsFromCategory()
    {
        Tournament tournament = new Tournament("Summer Cup");
        tournament.AddCategory(new TournamentCategory(6, 16, 800));
        tournament.AddCategory(new TournamentCategory(7, 16, 700));

        TournamentRegistration activeRegistration = new TournamentRegistration(
            tournament.Id,
            CreateVerifiedPair("Johann", "Franco", 6),
            6);

        TournamentRegistration cancelledRegistration = new TournamentRegistration(
            tournament.Id,
            CreateVerifiedPair("Bruno", "Joshua", 6),
            6);

        TournamentRegistration anotherCategoryRegistration = new TournamentRegistration(
            tournament.Id,
            CreateVerifiedPair("Lucas", "Mateo", 7),
            7);

        cancelledRegistration.Cancel();

        tournament.AddRegistration(activeRegistration);
        tournament.AddRegistration(cancelledRegistration);
        tournament.AddRegistration(anotherCategoryRegistration);

        IReadOnlyList<TournamentRegistration> result = tournament.GetActiveRegistrationsByCategory(6);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(activeRegistration.Id, result[0].Id);
    }

    [TestMethod]
    public void AddGroup_WithValidGroup_ShouldAddGroup()
    {
        Tournament tournament = new Tournament("Summer Cup");
        Group group = new Group("A", 6);

        tournament.AddGroup(group);

        Assert.AreEqual(1, tournament.Groups.Count);
        Assert.AreEqual(group.Id, tournament.Groups[0].Id);
    }

    [TestMethod]
    public void AddGroup_WithNullGroup_ShouldThrowArgumentNullException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<ArgumentNullException>(() => tournament.AddGroup(null!));
    }

    [TestMethod]
    public void AddGroup_WithSameGroup_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");
        Group group = new Group("A", 6);

        tournament.AddGroup(group);

        Assert.ThrowsException<ArgumentException>(() => tournament.AddGroup(group));
    }

    [TestMethod]
    public void AddGroup_WithSameNameAndCategory_ShouldThrowArgumentException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddGroup(new Group("A", 6));

        Assert.ThrowsException<ArgumentException>(() => tournament.AddGroup(new Group("A", 6)));
    }

    [TestMethod]
    public void AddGroup_WithSameNameButDifferentCategory_ShouldAddGroup()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.AddGroup(new Group("A", 6));
        tournament.AddGroup(new Group("A", 7));

        Assert.AreEqual(2, tournament.Groups.Count);
    }

    [TestMethod]
    public void RenameGroup_WithValidName_ShouldRenameGroup()
    {
        Tournament tournament = new Tournament("Summer Cup");
        Group group = new Group("A", 6);
        tournament.AddGroup(group);

        tournament.RenameGroup(group.Id, "B");

        Assert.AreEqual("B", group.Name);
    }

    [TestMethod]
    public void RemoveGroup_WithExistingGroup_ShouldRemoveGroup()
    {
        Tournament tournament = new Tournament("Summer Cup");
        Group group = new Group("A", 6);
        tournament.AddGroup(group);

        tournament.RemoveGroup(group.Id);

        Assert.AreEqual(0, tournament.Groups.Count);
    }

    [TestMethod]
    public void StartGroupStage_WhenDraft_ShouldSetStatusToGroupStageInProgress()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.StartGroupStage();

        Assert.AreEqual(TournamentStatus.GroupStageInProgress, tournament.Status);
        Assert.AreEqual("Fase de grupos", tournament.StatusLabel);
    }

    [TestMethod]
    public void StartKnockoutStage_WhenDraft_ShouldSetStatusToKnockoutInProgress()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.StartKnockoutStage();

        Assert.AreEqual(TournamentStatus.KnockoutInProgress, tournament.Status);
        Assert.AreEqual("Eliminatoria", tournament.StatusLabel);
    }

    [TestMethod]
    public void Finish_WhenGroupStageInProgress_ShouldSetStatusToFinished()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.StartGroupStage();
        tournament.Finish();

        Assert.AreEqual(TournamentStatus.Finished, tournament.Status);
        Assert.AreEqual("Finalizado", tournament.StatusLabel);
    }

    [TestMethod]
    public void Finish_WhenDraft_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        Assert.ThrowsException<InvalidOperationException>(() => tournament.Finish());
    }

    [TestMethod]
    public void Cancel_WhenDraft_ShouldSetStatusToCancelled()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.Cancel();

        Assert.AreEqual(TournamentStatus.Cancelled, tournament.Status);
        Assert.AreEqual("Cancelado", tournament.StatusLabel);
    }

    [TestMethod]
    public void Cancel_WhenFinished_ShouldThrowInvalidOperationException()
    {
        Tournament tournament = new Tournament("Summer Cup");

        tournament.StartGroupStage();
        tournament.Finish();

        Assert.ThrowsException<InvalidOperationException>(() => tournament.Cancel());
    }

    [TestMethod]
    public void Constructor_WithDetails_ShouldCreateTournamentWithClubLocationAndStartDate()
    {
        DateTime startDate = new DateTime(2026, 7, 15, 20, 30, 0);

        Tournament tournament = new Tournament(
            "Torneo Apertura",
            "Club Carrasco",
            "Montevideo",
            "Av. Italia 1234",
            startDate);

        Assert.AreEqual("Torneo Apertura", tournament.Name);
        Assert.AreEqual("Club Carrasco", tournament.ClubName);
        Assert.AreEqual("Montevideo", tournament.City);
        Assert.AreEqual("Av. Italia 1234", tournament.Address);
        Assert.AreEqual(startDate.Date, tournament.StartDate);
        Assert.IsNull(tournament.OrganizerId);
    }

    [TestMethod]
    public void Constructor_WithDetailsAndOrganizerId_ShouldCreateTournamentWithOrganizer()
    {
        Guid organizerId = Guid.NewGuid();

        Tournament tournament = new Tournament(
            "Torneo Apertura",
            "Club Carrasco",
            "Montevideo",
            "Av. Italia 1234",
            new DateTime(2026, 7, 15),
            organizerId);

        Assert.AreEqual(organizerId, tournament.OrganizerId);
        Assert.AreEqual("Club Carrasco", tournament.ClubName);
    }

    [TestMethod]
    public void Constructor_WithEmptyClubName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Tournament(
                "Torneo Apertura",
                "",
                "Montevideo",
                "Av. Italia 1234",
                new DateTime(2026, 7, 15)));
    }

    [TestMethod]
    public void UpdateDetails_WithValidData_ShouldUpdateClubLocationAndStartDate()
    {
        Tournament tournament = new Tournament("Torneo Apertura");
        DateTime startDate = new DateTime(2026, 8, 20, 19, 0, 0);

        tournament.UpdateDetails(
            "Club Prado",
            "Montevideo",
            "Camino Castro 900",
            startDate);

        Assert.AreEqual("Club Prado", tournament.ClubName);
        Assert.AreEqual("Montevideo", tournament.City);
        Assert.AreEqual("Camino Castro 900", tournament.Address);
        Assert.AreEqual(startDate.Date, tournament.StartDate);
    }

    private static Pair CreateVerifiedPair(string playerOneName, string playerTwoName, int category)
    {
        Player playerOne = CreateVerifiedPlayer(
            playerOneName,
            $"{playerOneName.ToLower()}@mail.com",
            category);

        Player playerTwo = CreateVerifiedPlayer(
            playerTwoName,
            $"{playerTwoName.ToLower()}@mail.com",
            category);

        return new Pair(playerOne, playerTwo);
    }

    private static Player CreateVerifiedPlayer(string name, string email, int category)
    {
        Player player = new Player(
            name,
            email,
            DominantHand.Right,
            PreferredSide.Drive,
            category);

        player.Verify();

        return player;
    }
}