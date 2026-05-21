using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class StandingService
{
    public List<Standing> CalculateStandings(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        var standings = group.Pairs
            .Select(pair => new Standing(pair))
            .ToDictionary(standing => standing.Pair.Id);

        foreach (var match in group.Matches)
        {
            if (!match.HasResult || match.Result == null)
                continue;

            var pairOneStanding = standings[match.PairOne.Id];
            var pairTwoStanding = standings[match.PairTwo.Id];

            var result = match.Result;

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