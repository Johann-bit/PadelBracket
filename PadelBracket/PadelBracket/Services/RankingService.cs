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
        return BuildRanking(null);
    }

    public List<RankingItem> GetRankingByCategory(int category)
    {
        ValidateCategory(category);

        return BuildRanking(category);
    }

    private List<RankingItem> BuildRanking(int? category)
    {
        Dictionary<(Guid PairId, int Category), RankingItem> ranking = new();

        IEnumerable<Group> groups = tournamentService
            .GetAllTournaments()
            .SelectMany(tournament => tournament.Groups);

        if (category.HasValue)
        {
            groups = groups.Where(group => group.Category == category.Value);
        }

        foreach (Group group in groups)
        {
            List<Match> completedMatches = group.Matches
                .Where(match =>
                    match.HasResult &&
                    match.Result is not null &&
                    match.Winner is not null &&
                    match.Loser is not null)
                .ToList();

            foreach (Match match in completedMatches)
            {
                EnsurePairExists(ranking, match.PairOne, group.Category);
                EnsurePairExists(ranking, match.PairTwo, group.Category);

                RankingItem pairOneRanking = ranking[(match.PairOne.Id, group.Category)];
                RankingItem pairTwoRanking = ranking[(match.PairTwo.Id, group.Category)];

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
        }

        return ranking
            .Values
            .OrderBy(item => item.Category)
            .ThenByDescending(item => item.Points)
            .ThenByDescending(item => item.Wins)
            .ThenByDescending(item => item.GameDifference)
            .ThenByDescending(item => item.GamesWon)
            .ThenBy(item => item.PairName)
            .ToList();
    }

    private static void EnsurePairExists(
        Dictionary<(Guid PairId, int Category), RankingItem> ranking,
        Pair pair,
        int category)
    {
        var key = (pair.Id, category);

        if (ranking.ContainsKey(key))
            return;

        ranking[key] = new RankingItem
        {
            PairId = pair.Id,
            PairName = pair.DisplayName,
            Category = category
        };
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }
}