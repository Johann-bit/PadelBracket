using PadelBracket.Domain.Entities;

namespace PadelBracket.Domain.Repositories;

public interface ITournamentRepository
{
    List<Tournament> GetAll();
    Tournament? GetById(Guid id);
    void Add(Tournament tournament);
    void Delete(Tournament tournament);
    bool ExistsByName(string name);
    bool ExistsByNameExceptId(string name, Guid id);
}