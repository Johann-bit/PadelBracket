using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class MatchHistoryService
{
    private readonly TournamentService tournamentService;

    public MatchHistoryService(TournamentService tournamentService)
    {
        this.tournamentService = tournamentService;
    }

    public List<MatchHistoryItem> GetCompletedGroupMatches()
    {
        return tournamentService
            .GetAllTournaments()
            .SelectMany(tournament => tournament.Groups.SelectMany(group =>
                group.Matches
                    .Where(match => match.HasResult && match.Result is not null && match.Winner is not null && match.Loser is not null)
                    .Select(match => new MatchHistoryItem
                    {
                        TournamentId = tournament.Id,
                        TournamentName = tournament.Name,
                        GroupName = group.Name,
                        CategoryLabel = group.CategoryLabel,
                        PairOneName = match.PairOne.DisplayName,
                        PairTwoName = match.PairTwo.DisplayName,
                        WinnerName = match.Winner!.DisplayName,
                        LoserName = match.Loser!.DisplayName,
                        Score = match.Result!.DisplayScore,
                        TournamentCreatedAt = tournament.CreatedAt
                    })
            ))
            .OrderByDescending(match => match.TournamentCreatedAt)
            .ThenBy(match => match.TournamentName)
            .ThenBy(match => match.GroupName)
            .ToList();
    }
}