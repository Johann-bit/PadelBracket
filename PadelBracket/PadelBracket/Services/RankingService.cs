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
        return BuildPairRanking(null);
    }

    public List<RankingItem> GetRankingByCategory(int category)
    {
        ValidateCategory(category);

        return BuildPairRanking(category);
    }

    public List<PlayerRankingItem> GetPlayerRanking()
    {
        return BuildPlayerRanking(null);
    }

    public List<PlayerRankingItem> GetPlayerRankingByCategory(int category)
    {
        ValidateCategory(category);

        return BuildPlayerRanking(category);
    }

    private List<RankingItem> BuildPairRanking(int? category)
    {
        Dictionary<(Guid PairId, int Category), RankingItem> ranking = new();

        IEnumerable<Group> groups = GetGroups(category);

        foreach (Group group in groups)
        {
            foreach (Match match in GetCompletedMatches(group))
            {
                EnsurePairExists(ranking, match.PairOne, group.Category);
                EnsurePairExists(ranking, match.PairTwo, group.Category);

                RankingItem pairOneRanking = ranking[(match.PairOne.Id, group.Category)];
                RankingItem pairTwoRanking = ranking[(match.PairTwo.Id, group.Category)];

                AddPairMatchStats(match, pairOneRanking, pairTwoRanking);
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

    private List<PlayerRankingItem> BuildPlayerRanking(int? category)
    {
        Dictionary<(Guid PlayerId, int Category), PlayerRankingItem> ranking = new();

        IEnumerable<Group> groups = GetGroups(category);

        foreach (Group group in groups)
        {
            foreach (Match match in GetCompletedMatches(group))
            {
                EnsurePlayerExists(ranking, match.PairOne.PlayerOne, group.Category);
                EnsurePlayerExists(ranking, match.PairOne.PlayerTwo, group.Category);
                EnsurePlayerExists(ranking, match.PairTwo.PlayerOne, group.Category);
                EnsurePlayerExists(ranking, match.PairTwo.PlayerTwo, group.Category);

                PlayerRankingItem pairOnePlayerOneRanking = ranking[(match.PairOne.PlayerOne.Id, group.Category)];
                PlayerRankingItem pairOnePlayerTwoRanking = ranking[(match.PairOne.PlayerTwo.Id, group.Category)];
                PlayerRankingItem pairTwoPlayerOneRanking = ranking[(match.PairTwo.PlayerOne.Id, group.Category)];
                PlayerRankingItem pairTwoPlayerTwoRanking = ranking[(match.PairTwo.PlayerTwo.Id, group.Category)];

                AddPlayerMatchStats(
                    match,
                    pairOnePlayerOneRanking,
                    pairOnePlayerTwoRanking,
                    pairTwoPlayerOneRanking,
                    pairTwoPlayerTwoRanking);
            }
        }

        return ranking
            .Values
            .OrderBy(item => item.Category)
            .ThenByDescending(item => item.Points)
            .ThenByDescending(item => item.Wins)
            .ThenByDescending(item => item.GameDifference)
            .ThenByDescending(item => item.GamesWon)
            .ThenBy(item => item.PlayerName)
            .ToList();
    }

    private IEnumerable<Group> GetGroups(int? category)
    {
        IEnumerable<Group> groups = tournamentService
            .GetAllTournaments()
            .SelectMany(tournament => tournament.Groups);

        if (category.HasValue)
        {
            groups = groups.Where(group => group.Category == category.Value);
        }

        return groups;
    }

    private static List<Match> GetCompletedMatches(Group group)
    {
        return group.Matches
            .Where(match =>
                match.HasResult &&
                match.Result is not null &&
                match.Winner is not null &&
                match.Loser is not null)
            .ToList();
    }

    private static void AddPairMatchStats(
        Match match,
        RankingItem pairOneRanking,
        RankingItem pairTwoRanking)
    {
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

    private static void AddPlayerMatchStats(
        Match match,
        PlayerRankingItem pairOnePlayerOneRanking,
        PlayerRankingItem pairOnePlayerTwoRanking,
        PlayerRankingItem pairTwoPlayerOneRanking,
        PlayerRankingItem pairTwoPlayerTwoRanking)
    {
        AddPlayerParticipationStats(
            pairOnePlayerOneRanking,
            match.Result!.PairOneGames,
            match.Result.PairTwoGames);

        AddPlayerParticipationStats(
            pairOnePlayerTwoRanking,
            match.Result.PairOneGames,
            match.Result.PairTwoGames);

        AddPlayerParticipationStats(
            pairTwoPlayerOneRanking,
            match.Result.PairTwoGames,
            match.Result.PairOneGames);

        AddPlayerParticipationStats(
            pairTwoPlayerTwoRanking,
            match.Result.PairTwoGames,
            match.Result.PairOneGames);

        if (match.Winner!.Id == match.PairOne.Id)
        {
            AddPlayerWin(pairOnePlayerOneRanking);
            AddPlayerWin(pairOnePlayerTwoRanking);
            AddPlayerLoss(pairTwoPlayerOneRanking);
            AddPlayerLoss(pairTwoPlayerTwoRanking);
        }
        else
        {
            AddPlayerWin(pairTwoPlayerOneRanking);
            AddPlayerWin(pairTwoPlayerTwoRanking);
            AddPlayerLoss(pairOnePlayerOneRanking);
            AddPlayerLoss(pairOnePlayerTwoRanking);
        }
    }

    private static void AddPlayerParticipationStats(
        PlayerRankingItem ranking,
        int gamesWon,
        int gamesLost)
    {
        ranking.MatchesPlayed++;
        ranking.GamesWon += gamesWon;
        ranking.GamesLost += gamesLost;
    }

    private static void AddPlayerWin(PlayerRankingItem ranking)
    {
        ranking.Wins++;
        ranking.Points += 3;
    }

    private static void AddPlayerLoss(PlayerRankingItem ranking)
    {
        ranking.Losses++;
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

    private static void EnsurePlayerExists(
        Dictionary<(Guid PlayerId, int Category), PlayerRankingItem> ranking,
        Player player,
        int category)
    {
        var key = (player.Id, category);

        if (ranking.ContainsKey(key))
            return;

        ranking[key] = new PlayerRankingItem
        {
            PlayerId = player.Id,
            PlayerName = player.Name,
            Category = category
        };
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }
}