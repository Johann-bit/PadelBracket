using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Repositories;

namespace PadelBracket.Services;

public class PlayerService
{
    private readonly IPlayerRepository playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public IReadOnlyList<Player> GetAll()
    {
        return playerRepository.GetAll();
    }

    public IReadOnlyList<PlayerDto> GetAllDtos()
    {
        return playerRepository.GetAll()
            .Select(player => new PlayerDto
            {
                Id = player.Id,
                Name = player.Name
            })
            .ToList();
    }

    public Player Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name is required.");

        if (playerRepository.ExistsByName(name))
            throw new ArgumentException("A player with that name already exists.");

        Player player = new Player(name);

        playerRepository.Add(player);

        return player;
    }

    public Player? GetById(Guid id)
    {
        return playerRepository.GetById(id);
    }

    public PlayerDto? GetDtoById(Guid id)
    {
        Player? player = GetById(id);

        if (player == null)
            return null;

        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name
        };
    }

    public void Rename(Guid id, string name)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        if (playerRepository.ExistsByNameExceptId(name, id))
            throw new ArgumentException("A player with that name already exists.");

        player.Rename(name);
    }

    public void Delete(Guid id)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        playerRepository.Delete(player);
    }

    public void SeedDefaultPlayers()
    {
        if (playerRepository.HasAny())
            return;

        Add("Johann");
        Add("Franco");
        Add("Bruno");
        Add("Joshua");
    }
}