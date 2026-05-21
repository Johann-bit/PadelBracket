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

            if (match.Result.PairOneWon)
            {
                pairOneStanding.AddWin(
                    match.Result.PairOneGames,
                    match.Result.PairTwoGames
                );

                pairTwoStanding.AddLoss(
                    match.Result.PairTwoGames,
                    match.Result.PairOneGames
                );
            }
            else
            {
                pairTwoStanding.AddWin(
                    match.Result.PairTwoGames,
                    match.Result.PairOneGames
                );

                pairOneStanding.AddLoss(
                    match.Result.PairOneGames,
                    match.Result.PairTwoGames
                );
            }
        }

        return standings.Values
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.GameDifference)
            .ThenByDescending(s => s.GamesFor)
            .ThenBy(s => s.GamesAgainst)
            .ToList();
    }
}