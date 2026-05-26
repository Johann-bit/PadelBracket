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
        Dictionary<Guid, RankingItem> ranking = new();

        List<Match> completedMatches = tournamentService
            .GetAllTournaments()
            .SelectMany(tournament => tournament.Groups)
            .SelectMany(group => group.Matches)
            .Where(match => match.HasResult && match.Result is not null && match.Winner is not null && match.Loser is not null)
            .ToList();

        foreach (Match match in completedMatches)
        {
            EnsurePairExists(ranking, match.PairOne);
            EnsurePairExists(ranking, match.PairTwo);

            RankingItem pairOneRanking = ranking[match.PairOne.Id];
            RankingItem pairTwoRanking = ranking[match.PairTwo.Id];

            pairOneRanking.MatchesPlayed++;
            pairTwoRanking.MatchesPlayed++;

            pairOneRanking.PairName = match.PairOne.DisplayName;
            pairTwoRanking.PairName = match.PairTwo.DisplayName;

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

    private static void EnsurePairExists(Dictionary<Guid, RankingItem> ranking, Pair pair)
    {
        if (ranking.ContainsKey(pair.Id))
            return;

        ranking[pair.Id] = new RankingItem
        {
            PairId = pair.Id,
            PairName = pair.DisplayName
        };
    }
}