using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryTournamentRepository : ITournamentRepository
{
    private readonly List<Tournament> tournaments = new();

    public List<Tournament> GetAll()
    {
        return tournaments;
    }

    public Tournament? GetById(Guid id)
    {
        return tournaments.FirstOrDefault(tournament => tournament.Id == id);
    }

    public void Add(Tournament tournament)
    {
        tournaments.Add(tournament);
    }

    public void AddRegistration(TournamentRegistration registration)
    {
    }

    public void AddGroup(Guid tournamentId, Group group)
    {
        Tournament tournament = tournaments.First(tournament => tournament.Id == tournamentId);

        tournament.AddGroup(group);
    }

    public void AddGroupMatches(Guid tournamentId, Guid groupId, List<Match> matches, TournamentStatus tournamentStatus)
    {
    }

    public void Delete(Tournament tournament)
    {
        tournaments.Remove(tournament);
    }

    public bool ExistsByName(string name)
    {
        return tournaments.Any(tournament =>
            string.Equals(tournament.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool ExistsByNameExceptId(string name, Guid id)
    {
        return tournaments.Any(tournament =>
            tournament.Id != id &&
            string.Equals(tournament.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void SaveChanges()
    {
    }
}
