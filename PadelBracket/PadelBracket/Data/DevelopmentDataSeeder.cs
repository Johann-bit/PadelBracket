using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Data;

public static class DevelopmentDataSeeder
{
    private const string DemoPassword = "Demo#2026";

    public static void ResetAndSeed(ApplicationDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        Seed(dbContext);
    }

    public static void Seed(ApplicationDbContext dbContext)
    {
        if (dbContext.Players.Any() ||
            dbContext.Organizers.Any() ||
            dbContext.Tournaments.Any())
        {
            return;
        }

        Organizer organizer = new(
            "Camila Pereyra",
            "organizador@arenapadel.test",
            "Club Cordon",
            "Montevideo",
            "099 123 456");

        dbContext.Organizers.Add(organizer);
        dbContext.OrganizerAccounts.Add(OrganizerAccount.Create(
            organizer.Id,
            organizer.Email,
            DemoPassword));

        Tournament tournament = new(
            "Copa Cordon Demo",
            organizer.ClubName,
            organizer.City,
            "Av Italia 6969",
            DateTime.Today.AddDays(21),
            organizer.Id);

        tournament.AddCategory(new TournamentCategory(5, 16, 900));
        tournament.AddCategory(new TournamentCategory(6, 16, 850));
        tournament.AddCategory(new TournamentCategory(7, 16, 800));
        tournament.AddCategory(new TournamentCategory(8, 16, 750));

        Player johann = CreatePlayer(
            dbContext,
            "Johann Rosas",
            "johann@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Backhand,
            5);

        Player joshua = CreatePlayer(
            dbContext,
            "Joshua Rosas",
            "joshua@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Drive,
            5);

        Player josi = CreatePlayer(
            dbContext,
            "Josi Perez",
            "josi@arenapadel.test",
            DominantHand.Left,
            PreferredSide.Drive,
            6);

        Player mateo = CreatePlayer(
            dbContext,
            "Mateo Silva",
            "mateo@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Backhand,
            6);

        Player lucia = CreatePlayer(
            dbContext,
            "Lucia Fernandez",
            "lucia@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Drive,
            7);

        Player valentina = CreatePlayer(
            dbContext,
            "Valentina Castro",
            "valentina@arenapadel.test",
            DominantHand.Left,
            PreferredSide.Backhand,
            7);

        Player diego = CreatePlayer(
            dbContext,
            "Diego Alvarez",
            "diego@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Drive,
            8);

        Player bruno = CreatePlayer(
            dbContext,
            "Bruno Rodriguez",
            "bruno@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Backhand,
            8);

        CreatePlayer(
            dbContext,
            "Nicolas Martinez",
            "nicolas@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Both,
            6);

        Pair pairOne = new(johann, joshua);
        Pair pairTwo = new(josi, mateo);
        Pair pairThree = new(lucia, valentina);
        Pair pairFour = new(diego, bruno);

        dbContext.Pairs.AddRange(pairOne, pairTwo, pairThree, pairFour);

        TournamentRegistration pendingRegistration = new(
            tournament.Id,
            pairOne,
            5);

        TournamentRegistration confirmedRegistration = new(
            tournament.Id,
            pairTwo,
            6);
        confirmedRegistration.Confirm();
        confirmedRegistration.MarkAsPaid();

        TournamentRegistration secondPendingRegistration = new(
            tournament.Id,
            pairThree,
            7);

        tournament.AddRegistration(pendingRegistration);
        tournament.AddRegistration(confirmedRegistration);
        tournament.AddRegistration(secondPendingRegistration);

        dbContext.Tournaments.Add(tournament);
        dbContext.SaveChanges();
    }

    private static Player CreatePlayer(
        ApplicationDbContext dbContext,
        string name,
        string email,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        Player player = new(
            name,
            email,
            dominantHand,
            preferredSide,
            category);

        dbContext.Players.Add(player);
        dbContext.PlayerAccounts.Add(PlayerAccount.Create(
            player.Id,
            player.Email,
            DemoPassword));

        return player;
    }
}
