using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Repositories;

namespace PadelBracket.Services;

public class TournamentService
{
    private readonly ITournamentRepository tournamentRepository;
    private readonly PairService pairService;

    public TournamentService(
        ITournamentRepository tournamentRepository,
        PairService pairService)
    {
        this.tournamentRepository = tournamentRepository;
        this.pairService = pairService;
    }

    public Tournament CreateTournament(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        if (tournamentRepository.ExistsByName(name))
            throw new ArgumentException("A tournament with the same name already exists.");

        var tournament = new Tournament(name);

        tournamentRepository.Add(tournament);

        return tournament;
    }

    public List<Tournament> GetAllTournaments()
    {
        return tournamentRepository.GetAll();
    }

    public Tournament? GetTournamentById(Guid tournamentId)
    {
        return tournamentRepository.GetById(tournamentId);
    }

    public void RenameTournament(Guid tournamentId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        var tournament = GetTournamentOrThrow(tournamentId);

        if (tournamentRepository.ExistsByNameExceptId(name, tournamentId))
            throw new ArgumentException("A tournament with the same name already exists.");

        tournament.Rename(name);
    }

    public void DeleteTournament(Guid tournamentId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournamentRepository.Delete(tournament);
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

        var group = tournament.Groups.FirstOrDefault(group => group.Id == groupId);

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

        var group = tournament.Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        return group;
    }

    private Match GetMatchOrThrow(Guid tournamentId, Guid groupId, Guid matchId)
    {
        var group = GetGroupOrThrow(tournamentId, groupId);

        var match = group.Matches.FirstOrDefault(match => match.Id == matchId);

        if (match == null)
            throw new ArgumentException("Match not found.");

        return match;
    }
}