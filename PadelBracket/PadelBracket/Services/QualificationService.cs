using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class QualificationService
{
    public List<PairQualification> CalculateQualifications(Group group, int qualifiedPairsCount)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        if (qualifiedPairsCount <= 0)
            throw new ArgumentException("Qualified pairs count must be greater than zero.");

        if (qualifiedPairsCount >= group.Pairs.Count)
            throw new ArgumentException("Qualified pairs count must be lower than the total number of pairs.");

        var currentStandings = CalculateStandings(group, new Dictionary<Guid, MatchResult>());

        var currentPositions = currentStandings
            .Select((standing, index) => new
            {
                PairId = standing.Pair.Id,
                Position = index + 1
            })
            .ToDictionary(item => item.PairId, item => item.Position);

        var pendingMatches = group.Matches
            .Where(match => !match.HasResult)
            .ToList();

        var possibleScenarios = GeneratePossibleScenarios(pendingMatches);

        var qualifications = new List<PairQualification>();

        foreach (var pair in group.Pairs)
        {
            var canQualify = false;
            var canBeEliminated = false;

            foreach (var scenario in possibleScenarios)
            {
                var simulatedStandings = CalculateStandings(group, scenario);

                var position = simulatedStandings.FindIndex(standing => standing.Pair.Id == pair.Id);

                if (position < 0)
                    continue;

                var isQualifiedInScenario = position < qualifiedPairsCount;

                if (isQualifiedInScenario)
                    canQualify = true;
                else
                    canBeEliminated = true;

                if (canQualify && canBeEliminated)
                    break;
            }

            var currentPosition = currentPositions[pair.Id];
            var isCurrentlyInZone = currentPosition <= qualifiedPairsCount;

            var status = GetQualificationStatus(
                canQualify,
                canBeEliminated,
                isCurrentlyInZone
            );

            qualifications.Add(new PairQualification(pair, status, currentPosition));
        }

        return qualifications
            .OrderBy(qualification => qualification.CurrentPosition)
            .ToList();
    }

    private static QualificationStatus GetQualificationStatus(
        bool canQualify,
        bool canBeEliminated,
        bool isCurrentlyInZone)
    {
        if (canQualify && !canBeEliminated)
            return QualificationStatus.MathematicallyQualified;

        if (!canQualify && canBeEliminated)
            return QualificationStatus.MathematicallyEliminated;

        if (isCurrentlyInZone)
            return QualificationStatus.InQualificationZone;

        return QualificationStatus.Alive;
    }

    private static List<Dictionary<Guid, MatchResult>> GeneratePossibleScenarios(List<Match> pendingMatches)
    {
        var scenarios = new List<Dictionary<Guid, MatchResult>>();

        GenerateScenarioRecursive(
            pendingMatches,
            0,
            new Dictionary<Guid, MatchResult>(),
            scenarios
        );

        return scenarios;
    }

    private static void GenerateScenarioRecursive(
        List<Match> pendingMatches,
        int index,
        Dictionary<Guid, MatchResult> currentScenario,
        List<Dictionary<Guid, MatchResult>> scenarios)
    {
        if (index == pendingMatches.Count)
        {
            scenarios.Add(new Dictionary<Guid, MatchResult>(currentScenario));
            return;
        }

        var match = pendingMatches[index];

        foreach (var result in CreatePossibleResults())
        {
            currentScenario[match.Id] = result;

            GenerateScenarioRecursive(
                pendingMatches,
                index + 1,
                currentScenario,
                scenarios
            );

            currentScenario.Remove(match.Id);
        }
    }

    private static List<MatchResult> CreatePossibleResults()
    {
        return new List<MatchResult>
        {
            // Pair one wins 2-0
            new MatchResult(new List<MatchSet>
            {
                new MatchSet(6, 4),
                new MatchSet(6, 4)
            }),

            // Pair one wins 2-1
            new MatchResult(new List<MatchSet>
            {
                new MatchSet(6, 4),
                new MatchSet(4, 6),
                new MatchSet(11, 9, true)
            }),

            // Pair two wins 2-0
            new MatchResult(new List<MatchSet>
            {
                new MatchSet(4, 6),
                new MatchSet(4, 6)
            }),

            // Pair two wins 2-1
            new MatchResult(new List<MatchSet>
            {
                new MatchSet(4, 6),
                new MatchSet(6, 4),
                new MatchSet(9, 11, true)
            })
        };
    }

    private static List<Standing> CalculateStandings(
        Group group,
        Dictionary<Guid, MatchResult> simulatedResults)
    {
        var standings = group.Pairs
            .Select(pair => new Standing(pair))
            .ToDictionary(standing => standing.Pair.Id);

        foreach (var match in group.Matches)
        {
            MatchResult? result = null;

            if (match.HasResult)
            {
                result = match.Result;
            }
            else if (simulatedResults.ContainsKey(match.Id))
            {
                result = simulatedResults[match.Id];
            }

            if (result == null)
                continue;

            var pairOneStanding = standings[match.PairOne.Id];
            var pairTwoStanding = standings[match.PairTwo.Id];

            if (result.PairOneWon)
            {
                pairOneStanding.AddWin(
                    result.PairOneSetsWon,
                    result.PairTwoSetsWon,
                    result.PairOneGames,
                    result.PairTwoGames
                );

                pairTwoStanding.AddLoss(
                    result.PairTwoSetsWon,
                    result.PairOneSetsWon,
                    result.PairTwoGames,
                    result.PairOneGames
                );
            }
            else
            {
                pairTwoStanding.AddWin(
                    result.PairTwoSetsWon,
                    result.PairOneSetsWon,
                    result.PairTwoGames,
                    result.PairOneGames
                );

                pairOneStanding.AddLoss(
                    result.PairOneSetsWon,
                    result.PairTwoSetsWon,
                    result.PairOneGames,
                    result.PairTwoGames
                );
            }
        }

        return standings.Values
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.SetDifference)
            .ThenByDescending(s => s.SetsFor)
            .ThenByDescending(s => s.GameDifference)
            .ThenByDescending(s => s.GamesFor)
            .ThenBy(s => s.GamesAgainst)
            .ToList();
    }
}