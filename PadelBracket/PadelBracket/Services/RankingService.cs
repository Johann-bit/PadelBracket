using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class RankingService
{
    private readonly TournamentService tournamentService;

    public RankingService(TournamentService tournamentService)
    {
        this.tournamentService = tournamentService;
    }

    public List<RankingItem> GetRanking()
    {
        Dictionary<string, RankingItem> ranking = new();

        List<Match> completedMatches = tournamentService
            .GetAllTournaments()
            .SelectMany(tournament => tournament.Groups)
            .SelectMany(group => group.Matches)
            .Where(match => match.HasResult && match.Result is not null && match.Winner is not null && match.Loser is not null)
            .ToList();

        foreach (Match match in completedMatches)
        {
            string pairOneName = match.PairOne.DisplayName;
            string pairTwoName = match.PairTwo.DisplayName;

            EnsurePairExists(ranking, pairOneName);
            EnsurePairExists(ranking, pairTwoName);

            RankingItem pairOneRanking = ranking[pairOneName];
            RankingItem pairTwoRanking = ranking[pairTwoName];

            pairOneRanking.MatchesPlayed++;
            pairTwoRanking.MatchesPlayed++;

            pairOneRanking.GamesWon += match.Result!.PairOneGames;
            pairOneRanking.GamesLost += match.Result.PairTwoGames;

            pairTwoRanking.GamesWon += match.Result.PairTwoGames;
            pairTwoRanking.GamesLost += match.Result.PairOneGames;

            if (match.Winner!.Id == match.PairOne.Id)
            {
                pairOneRanking.Wins++;
                pairOneRanking.Points += 3;
                pairTwoRanking.Losses++;
            }
            else
            {
                pairTwoRanking.Wins++;
                pairTwoRanking.Points += 3;
                pairOneRanking.Losses++;
            }
        }

        return ranking
            .Values
            .OrderByDescending(item => item.Points)
            .ThenByDescending(item => item.Wins)
            .ThenByDescending(item => item.GameDifference)
            .ThenByDescending(item => item.GamesWon)
            .ThenBy(item => item.PairName)
            .ToList();
    }

    private static void EnsurePairExists(Dictionary<string, RankingItem> ranking, string pairName)
    {
        if (ranking.ContainsKey(pairName))
            return;

        ranking[pairName] = new RankingItem
        {
            PairName = pairName
        };
    }
}