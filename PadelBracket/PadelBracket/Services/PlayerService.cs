using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class PlayerService
{
    private readonly List<Player> players = new();

    public IReadOnlyList<Player> GetAll()
    {
        return players;
    }

    public Player Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name is required.");

        if (players.Any(player => string.Equals(player.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("A player with that name already exists.");

        Player player = new Player(name);
        players.Add(player);

        return player;
    }

    public Player? GetById(Guid id)
    {
        return players.FirstOrDefault(player => player.Id == id);
    }

    public void Rename(Guid id, string name)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        if (players.Any(existingPlayer =>
                existingPlayer.Id != id &&
                string.Equals(existingPlayer.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A player with that name already exists.");
        }

        player.Rename(name);
    }

    public void Delete(Guid id)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        players.Remove(player);
    }

    public void SeedDefaultPlayers()
    {
        if (players.Any())
            return;

        Add("Johann");
        Add("Franco");
        Add("Bruno");
        Add("Joshua");
    }
}