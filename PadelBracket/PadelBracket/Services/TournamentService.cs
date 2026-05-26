using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class TournamentService
{
    private readonly List<Tournament> _tournaments = new();
    private readonly PairService pairService;

    public TournamentService() : this(new PairService())
    {
    }

    public TournamentService(PairService pairService)
    {
        this.pairService = pairService;
    }

    public Tournament CreateTournament(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        if (_tournaments.Any(tournament =>
                string.Equals(tournament.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A tournament with the same name already exists.");
        }

        var tournament = new Tournament(name);
        _tournaments.Add(tournament);

        return tournament;
    }

    public List<Tournament> GetAllTournaments()
    {
        return _tournaments;
    }

    public Tournament? GetTournamentById(Guid tournamentId)
    {
        return _tournaments.FirstOrDefault(t => t.Id == tournamentId);
    }

    public void RenameTournament(Guid tournamentId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        var tournament = GetTournamentOrThrow(tournamentId);

        if (_tournaments.Any(existingTournament =>
                existingTournament.Id != tournamentId &&
                string.Equals(existingTournament.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A tournament with the same name already exists.");
        }

        tournament.Rename(name);
    }

    public void DeleteTournament(Guid tournamentId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        _tournaments.Remove(tournament);
    }

    public void FinishTournament(Guid tournamentId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournament.Finish();
    }

    public void CancelTournament(Guid tournamentId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournament.Cancel();
    }

    public Group AddGroupToTournament(Guid tournamentId, string groupName, int category)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var group = new Group(groupName, category);
        tournament.AddGroup(group);

        return group;
    }

    public void RenameGroupInTournament(
        Guid tournamentId,
        Guid groupId,
        string groupName)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournament.RenameGroup(groupId, groupName);
    }

    public void DeleteGroupFromTournament(
        Guid tournamentId,
        Guid groupId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournament.RemoveGroup(groupId);
    }

    public Pair AddPairToGroup(
        Guid tournamentId,
        Guid groupId,
        string playerOneName,
        string playerTwoName)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        var playerOne = new Player(playerOneName);
        var playerTwo = new Player(playerTwoName);

        var pair = new Pair(playerOne, playerTwo);

        group.AddPair(pair);

        return pair;
    }

    public Pair AddExistingPairToGroup(
        Guid tournamentId,
        Guid groupId,
        Guid pairId)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        var pair = pairService.GetById(pairId)
            ?? throw new ArgumentException("Pair not found.");

        group.AddPair(pair);

        return pair;
    }

    public void UpdatePairInGroup(
        Guid tournamentId,
        Guid groupId,
        Guid pairId,
        string playerOneName,
        string playerTwoName)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        group.RenamePair(pairId, playerOneName, playerTwoName);
    }

    public void RemovePairFromGroup(
        Guid tournamentId,
        Guid groupId,
        Guid pairId)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        group.RemovePair(pairId);
    }

    public void GenerateGroupMatches(Guid tournamentId, Guid groupId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var group = tournament.Groups.FirstOrDefault(g => g.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        group.GenerateMatches();
        tournament.StartGroupStage();
    }

    public void RegisterMatchResult(
        Guid tournamentId,
        Guid groupId,
        Guid matchId,
        List<MatchSet> sets)
    {
        var match = GetMatchOrThrow(tournamentId, groupId, matchId);

        var result = new MatchResult(sets);

        match.RegisterResult(result);
    }

    public void ClearMatchResult(
        Guid tournamentId,
        Guid groupId,
        Guid matchId)
    {
        var match = GetMatchOrThrow(tournamentId, groupId, matchId);

        match.ClearResult();
    }

    private Tournament GetTournamentOrThrow(Guid tournamentId)
    {
        var tournament = GetTournamentById(tournamentId);

        if (tournament == null)
            throw new ArgumentException("Tournament not found.");

        return tournament;
    }

    private Group GetGroupOrThrow(Guid tournamentId, Guid groupId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var group = tournament.Groups.FirstOrDefault(g => g.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        return group;
    }

    private Match GetMatchOrThrow(Guid tournamentId, Guid groupId, Guid matchId)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        var match = group.Matches.FirstOrDefault(m => m.Id == matchId);

        if (match == null)
            throw new ArgumentException("Match not found.");

        return match;
    }
}