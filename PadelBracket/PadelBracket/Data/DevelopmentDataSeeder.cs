using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;

namespace PadelBracket.Data;

public static class DevelopmentDataSeeder
{
    private const string DemoPassword = "Demo#2026";
    private const int DemoPairsPerCategory = 32;
    private const int DemoPairsPerGroup = 4;
    private static readonly string[] PlayerFirstNames =
    {
        "Juan",
        "Nicolas",
        "Camilo",
        "Martin",
        "Federico",
        "Sebastian",
        "Rodrigo",
        "Agustin",
        "Santiago",
        "Mateo",
        "Bruno",
        "Diego",
        "Facundo",
        "Ignacio",
        "Lucas",
        "Tomas",
        "Matias",
        "Gonzalo",
        "Emiliano",
        "Joaquin",
        "Valentin",
        "Franco",
        "Pablo",
        "Andres",
        "Lucia",
        "Valentina",
        "Sofia",
        "Camila",
        "Florencia",
        "Martina",
        "Paula",
        "Agustina"
    };

    private static readonly string[] PlayerLastNames =
    {
        "Perez",
        "Rosas",
        "Suarez",
        "Acosta",
        "Lima",
        "Torres",
        "Mendez",
        "Sosa",
        "Silva",
        "Fernandez",
        "Castro",
        "Alvarez",
        "Rodriguez",
        "Martinez",
        "Gomez",
        "Varela",
        "Pereira",
        "Olivera",
        "Molina",
        "Cabrera",
        "Nunez",
        "Moreira",
        "Santos",
        "Ramos",
        "Ibarra",
        "Costa",
        "Machado",
        "Medina",
        "Herrera",
        "Barrios",
        "Cardozo",
        "Arias"
    };

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

        tournament.AddCategory(new TournamentCategory(5, DemoPairsPerCategory, 900));
        tournament.AddCategory(new TournamentCategory(6, DemoPairsPerCategory, 850));
        tournament.AddCategory(new TournamentCategory(7, DemoPairsPerCategory, 800));
        tournament.AddCategory(new TournamentCategory(8, DemoPairsPerCategory, 750));

        SeedCategoryDemo(dbContext, tournament, 5);
        SeedCategoryDemo(dbContext, tournament, 6);
        SeedCategoryDemo(dbContext, tournament, 7);
        SeedCategoryDemo(dbContext, tournament, 8);

        CreatePlayer(
            dbContext,
            "Nicolas Martinez",
            "nicolas.martinez@arenapadel.test",
            DominantHand.Right,
            PreferredSide.Both,
            6);

        tournament.StartGroupStage();

        dbContext.Tournaments.Add(tournament);
        dbContext.SaveChanges();
    }

    private static void SeedCategoryDemo(
        ApplicationDbContext dbContext,
        Tournament tournament,
        int category)
    {
        var pairs = new List<Pair>();

        for (int index = 0; index < DemoPairsPerCategory; index++)
        {
            Pair pair = CreatePair(dbContext, category, index);
            pairs.Add(pair);

            TournamentRegistration registration = new(
                tournament.Id,
                pair,
                category);

            registration.Confirm();
            registration.MarkAsPaid();

            tournament.AddRegistration(registration);
        }

        dbContext.Pairs.AddRange(pairs);

        for (int groupIndex = 0; groupIndex < DemoPairsPerCategory / DemoPairsPerGroup; groupIndex++)
        {
            string groupName = $"Grupo {GetGroupLetter(groupIndex)} - {GetCategoryLabel(category)}";

            AddCompletedGroup(
                tournament,
                groupName,
                category,
                pairs.Skip(groupIndex * DemoPairsPerGroup).Take(DemoPairsPerGroup).ToList());
        }
    }

    private static Pair CreatePair(
        ApplicationDbContext dbContext,
        int category,
        int pairIndex)
    {
        int firstPlayerIndex = (category - 5) * DemoPairsPerCategory * 2 + pairIndex * 2;
        int secondPlayerIndex = firstPlayerIndex + 1;

        Player playerOne = CreatePlayer(
            dbContext,
            CreatePlayerName(firstPlayerIndex),
            CreateEmail(category, firstPlayerIndex),
            firstPlayerIndex % 5 == 0 ? DominantHand.Left : DominantHand.Right,
            PreferredSide.Drive,
            category);

        Player playerTwo = CreatePlayer(
            dbContext,
            CreatePlayerName(secondPlayerIndex),
            CreateEmail(category, secondPlayerIndex),
            secondPlayerIndex % 6 == 0 ? DominantHand.Left : DominantHand.Right,
            PreferredSide.Backhand,
            category);

        return new Pair(playerOne, playerTwo);
    }

    private static string CreatePlayerName(int index)
    {
        string firstName = PlayerFirstNames[index % PlayerFirstNames.Length];
        string lastName = PlayerLastNames[
            (index * 7 + index / PlayerFirstNames.Length) % PlayerLastNames.Length];

        return $"{firstName} {lastName}";
    }

    private static string CreateEmail(int category, int index)
    {
        return $"jugador{category}_{index + 1}@arenapadel.test";
    }

    private static void AddCompletedGroup(
        Tournament tournament,
        string groupName,
        int category,
        List<Pair> pairs)
    {
        Group group = new(groupName, category);

        foreach (Pair pair in pairs)
        {
            group.AddPair(pair);
        }

        group.GenerateMatches();
        SeedGroupResults(group);

        tournament.AddGroup(group);
    }

    private static void SeedGroupResults(Group group)
    {
        foreach (Match match in group.Matches)
        {
            bool pairOneWins = group.Pairs.IndexOf(match.PairOne) < group.Pairs.IndexOf(match.PairTwo);

            match.RegisterResult(pairOneWins
                ? new MatchResult(new List<MatchSet>
                {
                    new(6, 3),
                    new(6, 4)
                })
                : new MatchResult(new List<MatchSet>
                {
                    new(3, 6),
                    new(4, 6)
                }));
        }
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

    private static string GetCategoryLabel(int category)
    {
        return category switch
        {
            1 => "1ra",
            2 => "2da",
            3 => "3ra",
            4 => "4ta",
            5 => "5ta",
            6 => "6ta",
            7 => "7ma",
            8 => "8va",
            _ => $"{category}ta"
        };
    }

    private static char GetGroupLetter(int groupIndex)
    {
        return (char)('A' + groupIndex);
    }
}
