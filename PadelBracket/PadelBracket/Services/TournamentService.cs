using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Services;

public class TournamentService
{
    private readonly ITournamentRepository tournamentRepository;
    private readonly PairService pairService;
    private readonly TournamentRegistrationService tournamentRegistrationService;

    public TournamentService(
        ITournamentRepository tournamentRepository,
        PairService pairService,
        TournamentRegistrationService tournamentRegistrationService)
    {
        this.tournamentRepository = tournamentRepository;
        this.pairService = pairService;
        this.tournamentRegistrationService = tournamentRegistrationService;
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

    public Tournament CreateTournament(string name, Guid organizerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        if (organizerId == Guid.Empty)
            throw new ArgumentException("Organizer id is required.");

        if (tournamentRepository.ExistsByName(name))
            throw new ArgumentException("A tournament with the same name already exists.");

        var tournament = new Tournament(name, organizerId);

        tournamentRepository.Add(tournament);

        return tournament;
    }

    public Tournament CreateTournament(
        string name,
        string clubName,
        string city,
        string address,
        DateTime startDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        if (tournamentRepository.ExistsByName(name))
            throw new ArgumentException("A tournament with the same name already exists.");

        var tournament = new Tournament(
            name,
            clubName,
            city,
            address,
            startDate);

        tournamentRepository.Add(tournament);

        return tournament;
    }

    public Tournament CreateTournament(
        string name,
        string clubName,
        string city,
        string address,
        DateTime startDate,
        Guid organizerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        if (organizerId == Guid.Empty)
            throw new ArgumentException("Organizer id is required.");

        if (tournamentRepository.ExistsByName(name))
            throw new ArgumentException("A tournament with the same name already exists.");

        var tournament = new Tournament(
            name,
            clubName,
            city,
            address,
            startDate,
            organizerId);

        tournamentRepository.Add(tournament);

        return tournament;
    }

    public List<Tournament> GetAllTournaments()
    {
        return tournamentRepository.GetAll();
    }

    public List<TournamentDto> GetAllTournamentDtos()
    {
        return tournamentRepository.GetAll()
            .Select(ToDto)
            .ToList();
    }

    public List<TournamentDto> GetTournamentDtosByOrganizerId(Guid organizerId)
    {
        if (organizerId == Guid.Empty)
            throw new ArgumentException("Organizer id is required.");

        return tournamentRepository.GetAll()
            .Where(tournament => tournament.OrganizerId == organizerId)
            .Select(ToDto)
            .ToList();
    }

    public Tournament? GetTournamentById(Guid tournamentId)
    {
        return tournamentRepository.GetById(tournamentId);
    }

    public TournamentDto? GetTournamentDtoById(Guid tournamentId)
    {
        Tournament? tournament = GetTournamentById(tournamentId);

        if (tournament == null)
            return null;

        return ToDto(tournament);
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

    public TournamentCategory AddCategoryToTournament(
        Guid tournamentId,
        int category,
        int maxPairs,
        decimal registrationFee)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var tournamentCategory = new TournamentCategory(
            category,
            maxPairs,
            registrationFee);

        tournament.AddCategory(tournamentCategory);

        return tournamentCategory;
    }

    public void RemoveCategoryFromTournament(Guid tournamentId, int category)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        tournament.RemoveCategory(category);
    }

    public TournamentRegistration RegisterPairToTournament(
        Guid tournamentId,
        Guid pairId,
        int category)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        Pair pair = pairService.GetById(pairId)
            ?? throw new ArgumentException("Pair not found.");

        return tournamentRegistrationService.RegisterPair(
            tournament,
            pair,
            category);
    }

    public void CancelRegistration(
        Guid tournamentId,
        Guid registrationId)
    {
        Tournament tournament = GetTournamentOrThrow(tournamentId);

        TournamentRegistration registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId)
            ?? throw new ArgumentException("Registration not found.");

        tournamentRegistrationService.CancelRegistration(registration);
    }

    public void ConfirmRegistration(
    Guid tournamentId,
    Guid registrationId)
    {
        Tournament tournament = GetTournamentOrThrow(tournamentId);

        TournamentRegistration registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId)
            ?? throw new ArgumentException("Registration not found.");

        tournamentRegistrationService.ConfirmRegistration(registration);
    }

    public void RejectRegistration(
        Guid tournamentId,
        Guid registrationId)
    {
        Tournament tournament = GetTournamentOrThrow(tournamentId);

        TournamentRegistration registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId)
            ?? throw new ArgumentException("Registration not found.");

        tournamentRegistrationService.RejectRegistration(registration);
    }

    public void MarkRegistrationAsPaid(
        Guid tournamentId,
        Guid registrationId)
    {
        Tournament tournament = GetTournamentOrThrow(tournamentId);

        TournamentRegistration registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId)
            ?? throw new ArgumentException("Registration not found.");

        tournamentRegistrationService.MarkRegistrationAsPaid(registration);
    }

    public void RefundRegistration(
        Guid tournamentId,
        Guid registrationId)
    {
        Tournament tournament = GetTournamentOrThrow(tournamentId);

        TournamentRegistration registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId)
            ?? throw new ArgumentException("Registration not found.");

        tournamentRegistrationService.RefundRegistration(registration);
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

    public Pair AddConfirmedRegistrationPairToGroup(
        Guid tournamentId,
        Guid groupId,
        Guid registrationId)
    {
        var tournament = GetTournamentOrThrow(tournamentId);

        var group = tournament.Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        var registration = tournament.Registrations
            .FirstOrDefault(registration => registration.Id == registrationId);

        if (registration == null)
            throw new ArgumentException("Registration not found.");

        if (registration.Status != RegistrationStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed registrations can be added to a group.");

        if (registration.Category != group.Category)
            throw new ArgumentException("Registration category must match group category.");

        group.AddPair(registration.Pair);

        return registration.Pair;
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

    private static TournamentDto ToDto(Tournament tournament)
    {
        return new TournamentDto
        {
            Id = tournament.Id,
            OrganizerId = tournament.OrganizerId,
            Name = tournament.Name,
            ClubName = tournament.ClubName,
            City = tournament.City,
            Address = tournament.Address,
            StartDate = tournament.StartDate,
            CreatedAt = tournament.CreatedAt,
            StatusLabel = tournament.StatusLabel,
            GroupCount = tournament.Groups.Count,
            Categories = tournament.TournamentCategories
                .Select(category =>
                    TournamentCategoryDto.FromEntity(
                        category,
                        tournament.GetActiveRegistrationsByCategory(category.Category).Count))
                .ToList(),
            Registrations = tournament.Registrations
                .Select(TournamentRegistrationDto.FromEntity)
                .ToList()
        };
    }
}