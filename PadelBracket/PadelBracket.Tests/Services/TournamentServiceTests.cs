using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories;
using PadelBracket.Services;

namespace PadelBracket.Tests.Services;

[TestClass]
public class TournamentServiceTests
{
    private static TournamentService CreateTournamentService()
    {
        return CreateTournamentContext().TournamentService;
    }

    private static (TournamentService TournamentService, PairService PairService) CreateTournamentContext()
    {
        var pairRepository = new InMemoryPairRepository();
        var tournamentRepository = new InMemoryTournamentRepository();
        var pairService = new PairService(pairRepository);
        var tournamentRegistrationService = new TournamentRegistrationService();

        var tournamentService = new TournamentService(
            tournamentRepository,
            pairService,
            tournamentRegistrationService);

        return (tournamentService, pairService);
    }

    [TestMethod]
    public void CreateTournament_WhenNameIsValid_AddsTournamentToList()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var tournaments = service.GetAllTournaments();

        Assert.AreEqual(1, tournaments.Count);
        Assert.AreEqual(tournament.Id, tournaments[0].Id);
        Assert.AreEqual("Torneo Apertura", tournaments[0].Name);
    }

    [TestMethod]
    public void CreateTournament_WithOrganizerId_ShouldAssignOrganizer()
    {
        var service = CreateTournamentService();
        Guid organizerId = Guid.NewGuid();

        var tournament = service.CreateTournament("Torneo Apertura", organizerId);

        Assert.AreEqual(organizerId, tournament.OrganizerId);
    }

    [TestMethod]
    public void CreateTournament_WithEmptyOrganizerId_ShouldThrowArgumentException()
    {
        var service = CreateTournamentService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.CreateTournament("Torneo Apertura", Guid.Empty));
    }

    [TestMethod]
    public void CreateTournament_WithDetails_ShouldCreateTournamentWithClubLocationAndStartDate()
    {
        var service = CreateTournamentService();
        DateTime startDate = new DateTime(2026, 7, 15);

        var tournament = service.CreateTournament(
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
    public void CreateTournament_WithDetailsAndOrganizerId_ShouldAssignOrganizer()
    {
        var service = CreateTournamentService();
        Guid organizerId = Guid.NewGuid();

        var tournament = service.CreateTournament(
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
    public void GetTournamentDtoById_WhenTournamentHasDetails_ReturnsDetails()
    {
        var service = CreateTournamentService();
        DateTime startDate = new DateTime(2026, 7, 15);
        Guid organizerId = Guid.NewGuid();

        var tournament = service.CreateTournament(
            "Torneo Apertura",
            "Club Carrasco",
            "Montevideo",
            "Av. Italia 1234",
            startDate,
            organizerId);

        var dto = service.GetTournamentDtoById(tournament.Id);

        Assert.IsNotNull(dto);
        Assert.AreEqual(organizerId, dto.OrganizerId);
        Assert.AreEqual("Club Carrasco", dto.ClubName);
        Assert.AreEqual("Montevideo", dto.City);
        Assert.AreEqual("Av. Italia 1234", dto.Address);
        Assert.AreEqual(startDate.Date, dto.StartDate);
    }

    [TestMethod]
    public void GetTournamentDtosByOrganizerId_ShouldReturnOnlyOrganizerTournaments()
    {
        var service = CreateTournamentService();
        Guid organizerId = Guid.NewGuid();
        Guid anotherOrganizerId = Guid.NewGuid();

        var organizerTournament = service.CreateTournament(
            "Torneo Apertura",
            organizerId);

        service.CreateTournament(
            "Torneo Clausura",
            anotherOrganizerId);

        var tournaments = service.GetTournamentDtosByOrganizerId(organizerId);

        Assert.AreEqual(1, tournaments.Count);
        Assert.AreEqual(organizerTournament.Id, tournaments[0].Id);
        Assert.AreEqual(organizerId, tournaments[0].OrganizerId);
    }

    [TestMethod]
    public void GetTournamentDtosByOrganizerId_WithEmptyOrganizerId_ShouldThrowArgumentException()
    {
        var service = CreateTournamentService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.GetTournamentDtosByOrganizerId(Guid.Empty));
    }

    [TestMethod]
    public void GetTournamentById_WhenTournamentExists_ReturnsTournament()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var foundTournament = service.GetTournamentById(tournament.Id);

        Assert.IsNotNull(foundTournament);
        Assert.AreEqual(tournament.Id, foundTournament.Id);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentExists_AddsGroup()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        Assert.AreEqual(1, tournament.Groups.Count);
        Assert.AreEqual(group.Id, tournament.Groups[0].Id);
        Assert.AreEqual("Group A", tournament.Groups[0].Name);
        Assert.AreEqual(5, tournament.Groups[0].Category);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsValid_AddsGroupWithCategory()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        Assert.AreEqual(5, group.Category);
        Assert.AreEqual("5ta categoría", group.CategoryLabel);
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsLowerThanOne_ThrowsArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(tournament.Id, "Group A", 0)
        );
    }

    [TestMethod]
    public void AddGroupToTournament_WhenCategoryIsGreaterThanEight_ThrowsArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(tournament.Id, "Group A", 9)
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupExists_AddsPair()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        var pair = service.AddPairToGroup(
            tournament.Id,
            group.Id,
            "Juan",
            "Pedro"
        );

        Assert.AreEqual(1, group.Pairs.Count);
        Assert.AreEqual(pair.Id, group.Pairs[0].Id);
        Assert.AreEqual("Juan / Pedro", group.Pairs[0].DisplayName);
    }

    [TestMethod]
    public void GenerateGroupMatches_WhenGroupHasThreePairs_GeneratesThreeMatches()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        Assert.AreEqual(3, group.Matches.Count);
    }

    [TestMethod]
    public void GenerateGroupMatches_WhenGroupHasTwoPairs_ThrowsInvalidOperationException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateGroupMatches(tournament.Id, group.Id));
    }

    [TestMethod]
    public void GenerateGroupMatches_WhenMatchesAlreadyExist_ThrowsExceptionAndKeepsExistingMatches()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        var originalMatchesCount = group.Matches.Count;
        var originalMatchIds = group.Matches.Select(match => match.Id).ToList();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateGroupMatches(tournament.Id, group.Id)
        );

        Assert.AreEqual(originalMatchesCount, group.Matches.Count);
        CollectionAssert.AreEqual(
            originalMatchIds,
            group.Matches.Select(match => match.Id).ToList()
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenMatchesAlreadyExist_ThrowsException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.AddPairToGroup(
                tournament.Id,
                group.Id,
                "Lucas",
                "Mati"
            )
        );

        Assert.AreEqual(3, group.Pairs.Count);
    }

    [TestMethod]
    public void RegisterMatchResult_WhenMatchExists_SavesResult()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");
        var group = service.AddGroupToTournament(tournament.Id, "Group A", 5);

        service.AddPairToGroup(tournament.Id, group.Id, "Juan", "Pedro");
        service.AddPairToGroup(tournament.Id, group.Id, "Nico", "Santi");
        service.AddPairToGroup(tournament.Id, group.Id, "Lucas", "Mati");

        service.GenerateGroupMatches(tournament.Id, group.Id);

        var match = group.Matches.First();

        service.RegisterMatchResult(
            tournament.Id,
            group.Id,
            match.Id,
            new List<MatchSet>
            {
                new MatchSet(6, 3),
                new MatchSet(6, 4)
            }
        );

        Assert.IsTrue(match.HasResult);
        Assert.IsNotNull(match.Result);
        Assert.AreEqual(12, match.Result.PairOneGames);
        Assert.AreEqual(7, match.Result.PairTwoGames);
        Assert.AreEqual(2, match.Result.PairOneSetsWon);
        Assert.AreEqual(0, match.Result.PairTwoSetsWon);
    }

    [TestMethod]
    public void CancelRegistration_WhenRegistrationExists_ShouldCancelRegistration()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.CancelRegistration(tournament.Id, registration.Id);

        Assert.AreEqual(RegistrationStatus.Cancelled, registration.Status);
        Assert.AreEqual(PaymentStatus.Cancelled, registration.PaymentStatus);
    }

    [TestMethod]
    public void CancelRegistration_WhenRegistrationDoesNotExist_ShouldThrowArgumentException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.CancelRegistration(tournament.Id, Guid.NewGuid()));
    }

    [TestMethod]
    public void AddGroupToTournament_WhenTournamentDoesNotExist_ThrowsException()
    {
        var service = CreateTournamentService();

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddGroupToTournament(Guid.NewGuid(), "Group A", 5)
        );
    }

    [TestMethod]
    public void AddPairToGroup_WhenGroupDoesNotExist_ThrowsException()
    {
        var service = CreateTournamentService();

        var tournament = service.CreateTournament("Torneo Apertura");

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddPairToGroup(
                tournament.Id,
                Guid.NewGuid(),
                "Juan",
                "Pedro"
            )
        );
    }

    [TestMethod]
    public void ConfirmRegistration_WhenRegistrationExists_ShouldConfirmRegistration()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.ConfirmRegistration(tournament.Id, registration.Id);

        Assert.AreEqual(RegistrationStatus.Confirmed, registration.Status);
    }

    [TestMethod]
    public void RejectRegistration_WhenRegistrationExists_ShouldRejectRegistration()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.RejectRegistration(tournament.Id, registration.Id);

        Assert.AreEqual(RegistrationStatus.Rejected, registration.Status);
    }

    [TestMethod]
    public void MarkRegistrationAsPaid_WhenRegistrationExists_ShouldMarkPaymentAsPaid()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.MarkRegistrationAsPaid(tournament.Id, registration.Id);

        Assert.AreEqual(PaymentStatus.Paid, registration.PaymentStatus);
    }

    [TestMethod]
    public void RefundRegistration_WhenRegistrationIsPaid_ShouldRefundPayment()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.MarkRegistrationAsPaid(tournament.Id, registration.Id);
        service.RefundRegistration(tournament.Id, registration.Id);

        Assert.AreEqual(PaymentStatus.Refunded, registration.PaymentStatus);
    }

    [TestMethod]
    public void AddConfirmedRegistrationPairToGroup_WhenRegistrationIsConfirmed_ShouldAddPairToGroup()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var group = service.AddGroupToTournament(tournament.Id, "Grupo A", 6);
        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.ConfirmRegistration(tournament.Id, registration.Id);

        var addedPair = service.AddConfirmedRegistrationPairToGroup(
            tournament.Id,
            group.Id,
            registration.Id);

        Assert.AreEqual(pair.Id, addedPair.Id);
        Assert.AreEqual(1, group.Pairs.Count);
        Assert.AreEqual(pair.Id, group.Pairs[0].Id);
    }

    [TestMethod]
    public void AddConfirmedRegistrationPairToGroup_WhenRegistrationIsPending_ShouldThrowInvalidOperationException()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);

        var group = service.AddGroupToTournament(tournament.Id, "Grupo A", 6);
        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.AddConfirmedRegistrationPairToGroup(
                tournament.Id,
                group.Id,
                registration.Id));

        Assert.AreEqual(0, group.Pairs.Count);
    }

    [TestMethod]
    public void AddConfirmedRegistrationPairToGroup_WhenRegistrationCategoryDoesNotMatchGroup_ShouldThrowArgumentException()
    {
        var context = CreateTournamentContext();
        var service = context.TournamentService;

        var tournament = service.CreateTournament("Torneo Apertura");
        service.AddCategoryToTournament(tournament.Id, 6, 16, 800);
        service.AddCategoryToTournament(tournament.Id, 5, 16, 800);

        var group = service.AddGroupToTournament(tournament.Id, "Grupo A", 5);
        var pair = CreateCompletePair(context.PairService, "Juan Perez", "Pedro Gomez", 6);
        var registration = service.RegisterPairToTournament(tournament.Id, pair.Id, 6);

        service.ConfirmRegistration(tournament.Id, registration.Id);

        Assert.ThrowsException<ArgumentException>(() =>
            service.AddConfirmedRegistrationPairToGroup(
                tournament.Id,
                group.Id,
                registration.Id));

        Assert.AreEqual(0, group.Pairs.Count);
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
}
