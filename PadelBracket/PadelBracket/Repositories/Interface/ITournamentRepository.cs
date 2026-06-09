using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface ITournamentRepository
{
    List<Tournament> GetAll();
    Tournament? GetById(Guid id);
    void Add(Tournament tournament);
    void AddRegistration(TournamentRegistration registration);
    void AddGroup(Guid tournamentId, Group group);
    void Delete(Tournament tournament);
    bool ExistsByName(string name);
    bool ExistsByNameExceptId(string name, Guid id);
    void SaveChanges();
}
