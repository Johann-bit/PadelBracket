using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class KnockoutService
{
    private readonly StandingService _standingService;
    private readonly List<KnockoutBracket> _brackets = new();

    public KnockoutService(StandingService standingService)
    {
        _standingService = standingService ?? throw new ArgumentNullException(nameof(standingService));
    }

    public KnockoutBracket? GetBracket(Guid tournamentId, int category)
    {
        return _brackets.FirstOrDefault(bracket =>
            bracket.TournamentId == tournamentId &&
            bracket.Category == category
        );
    }

    public KnockoutBracket GenerateBracket(
        Tournament tournament,
        int category,
        int qualifiedPairsPerGroup = 2)
    {
        if (tournament == null)
            throw new ArgumentNullException(nameof(tournament));

        if (category < 1 || category > 8)
            throw new ArgumentException("La categoría debe estar entre 1 y 8.");

        if (qualifiedPairsPerGroup != 2)
            throw new ArgumentException("Por ahora la llave solo soporta 2 clasificados por grupo.");

        var existingBracket = GetBracket(tournament.Id, category);

        if (existingBracket != null)
            return existingBracket;

        var categoryGroups = tournament.Groups
            .Where(group => group.Category == category)
            .OrderBy(group => group.Name)
            .ToList();

        ValidateCategoryGroups(categoryGroups, qualifiedPairsPerGroup);

        var qualifiedPairs = GetQualifiedPairsByGroup(categoryGroups, qualifiedPairsPerGroup);

        var totalQualifiedPairs = qualifiedPairs.Count;

        var matches = new List<KnockoutMatch>();

        AddFirstRoundMatches(matches, categoryGroups, qualifiedPairs, totalQualifiedPairs);
        AddPendingRounds(matches, totalQualifiedPairs);

        var bracket = new KnockoutBracket(tournament.Id, category, matches);

        _brackets.Add(bracket);

        return bracket;
    }

    public List<KnockoutMatch> GenerateSemifinals(
        Tournament tournament,
        int category,
        int qualifiedPairsPerGroup = 2)
    {
        var bracket = GenerateBracket(tournament, category, qualifiedPairsPerGroup);

        return bracket.Matches
            .Where(match => match.RoundName.StartsWith("Semifinal"))
            .ToList();
    }

    private void ValidateCategoryGroups(List<Group> categoryGroups, int qualifiedPairsPerGroup)
    {
        if (!categoryGroups.Any())
            throw new InvalidOperationException("No hay grupos para esta categoría.");

        if (!IsValidGroupCount(categoryGroups.Count))
        {
            throw new InvalidOperationException(
                "La cantidad de grupos debe ser 2, 4 u 8 para generar una llave eliminatoria."
            );
        }

        foreach (var group in categoryGroups)
        {
            ValidateGroupIsReady(group, qualifiedPairsPerGroup);
        }
    }

    private static bool IsValidGroupCount(int groupCount)
    {
        return groupCount == 2 || groupCount == 4 || groupCount == 8;
    }

    private static void ValidateGroupIsReady(Group group, int qualifiedPairsPerGroup)
    {
        if (group.Pairs.Count < qualifiedPairsPerGroup)
            throw new InvalidOperationException("Cada grupo debe tener suficientes parejas.");

        if (!group.Matches.Any())
            throw new InvalidOperationException("Todos los grupos deben tener partidos generados.");

        if (group.Matches.Any(match => !match.HasResult))
        {
            throw new InvalidOperationException(
                "Todos los partidos de fase de grupos deben tener resultado antes de generar la llave."
            );
        }
    }

    private List<QualifiedPair> GetQualifiedPairsByGroup(
        List<Group> groups,
        int qualifiedPairsPerGroup)
    {
        var qualifiedPairs = new List<QualifiedPair>();

        foreach (var group in groups)
        {
            var standings = _standingService.CalculateStandings(group);

            for (int i = 0; i < qualifiedPairsPerGroup; i++)
            {
                qualifiedPairs.Add(new QualifiedPair(
                    group,
                    standings[i].Pair,
                    i + 1
                ));
            }
        }

        return qualifiedPairs;
    }

    private static void AddFirstRoundMatches(
        List<KnockoutMatch> matches,
        List<Group> categoryGroups,
        List<QualifiedPair> qualifiedPairs,
        int totalQualifiedPairs)
    {
        var firstRoundName = GetFirstRoundName(totalQualifiedPairs);

        for (int i = 0; i < categoryGroups.Count; i++)
        {
            var group = categoryGroups[i];
            var oppositeGroup = categoryGroups[categoryGroups.Count - 1 - i];

            var groupWinner = qualifiedPairs.First(q =>
                q.Group.Id == group.Id &&
                q.Position == 1
            );

            var oppositeGroupRunnerUp = qualifiedPairs.First(q =>
                q.Group.Id == oppositeGroup.Id &&
                q.Position == 2
            );

            matches.Add(new KnockoutMatch(
                $"{firstRoundName} {i + 1}",
                groupWinner.Pair,
                oppositeGroupRunnerUp.Pair
            ));
        }
    }

    private static void AddPendingRounds(List<KnockoutMatch> matches, int totalQualifiedPairs)
    {
        var firstRoundMatchesCount = totalQualifiedPairs / 2;
        var nextRoundMatchesCount = firstRoundMatchesCount / 2;

        while (nextRoundMatchesCount >= 1)
        {
            var roundName = GetRoundNameByMatchCount(nextRoundMatchesCount);

            for (int i = 1; i <= nextRoundMatchesCount; i++)
            {
                matches.Add(new KnockoutMatch(
                    $"{roundName} {i}",
                    null,
                    null
                ));
            }

            nextRoundMatchesCount /= 2;
        }
    }

    private static string GetFirstRoundName(int totalQualifiedPairs)
    {
        return totalQualifiedPairs switch
        {
            4 => "Semifinal",
            8 => "Cuarto de final",
            16 => "Octavo de final",
            _ => throw new InvalidOperationException("Cantidad de clasificados no soportada.")
        };
    }

    private static string GetRoundNameByMatchCount(int matchCount)
    {
        return matchCount switch
        {
            1 => "Final",
            2 => "Semifinal",
            4 => "Cuarto de final",
            8 => "Octavo de final",
            _ => "Ronda"
        };
    }

    private class QualifiedPair
    {
        public Group Group { get; }
        public Pair Pair { get; }
        public int Position { get; }

        public QualifiedPair(Group group, Pair pair, int position)
        {
            Group = group;
            Pair = pair;
            Position = position;
        }
    }
}