using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryPlayerRepository : IPlayerRepository
{
    private readonly List<Player> players = new();

    public IReadOnlyList<Player> GetAll()
    {
        return players;
    }

    public Player? GetById(Guid id)
    {
        return players.FirstOrDefault(player => player.Id == id);
    }

    public void Add(Player player)
    {
        players.Add(player);
    }

    public void Delete(Player player)
    {
        players.Remove(player);
    }

    public bool ExistsByName(string name)
    {
        return players.Any(player =>
            string.Equals(player.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool ExistsByNameExceptId(string name, Guid id)
    {
        return players.Any(player =>
            player.Id != id &&
            string.Equals(player.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool HasAny()
    {
        return players.Any();
    }

    public void SaveChanges()
    {
    }
}