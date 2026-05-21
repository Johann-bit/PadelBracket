using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class TournamentService
{
    private readonly List<Tournament> _tournaments = new();

    public Tournament CreateTournament(string name)
    {
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

    public Group AddGroupToTournament(Guid tournamentId, string groupName)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var group = new Group(groupName);
        tournament.AddGroup(group);

        return group;
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

    public void GenerateGroupMatches(Guid tournamentId, Guid groupId)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        group.GenerateMatches();
    }

    public void RegisterMatchResult(
        Guid tournamentId,
        Guid groupId,
        Guid matchId,
        List<MatchSet> sets)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        var match = group.Matches.FirstOrDefault(m => m.Id == matchId);

        if (match == null)
            throw new ArgumentException("Match not found.");

        var result = new MatchResult(sets);

        match.RegisterResult(result);
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
}