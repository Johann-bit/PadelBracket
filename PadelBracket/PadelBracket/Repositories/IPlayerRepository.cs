using PadelBracket.Domain.Entities;

namespace PadelBracket.Domain.Repositories;

public interface IPlayerRepository
{
    IReadOnlyList<Player> GetAll();
    Player? GetById(Guid id);
    void Add(Player player);
    void Delete(Player player);
    bool ExistsByName(string name);
    bool ExistsByNameExceptId(string name, Guid id);
    bool HasAny();
}