using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Enums;
using PadelBracket.Repositories.Interface;

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
            .Select(ToDto)
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

    public Player Add(
        string name,
        string email,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name is required.");

        if (playerRepository.ExistsByName(name))
            throw new ArgumentException("A player with that name already exists.");

        if (EmailAlreadyExists(email))
            throw new ArgumentException("A player with that email already exists.");

        Player player = new Player(
            name,
            email,
            dominantHand,
            preferredSide,
            category);

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

        return ToDto(player);
    }

    public void Rename(Guid id, string name)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        if (playerRepository.ExistsByNameExceptId(name, id))
            throw new ArgumentException("A player with that name already exists.");

        player.Rename(name);
    }

    public void UpdatePersonalData(Guid id, string name, string email)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        if (playerRepository.ExistsByNameExceptId(name, id))
            throw new ArgumentException("A player with that name already exists.");

        if (EmailAlreadyExistsExceptId(email, id))
            throw new ArgumentException("A player with that email already exists.");

        player.UpdatePersonalData(name, email);
    }

    public void UpdateSportData(
        Guid id,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        player.UpdateSportData(dominantHand, preferredSide, category);
    }

    public void Verify(Guid id)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        player.Verify();
    }

    public void RejectVerification(Guid id)
    {
        Player player = GetById(id) ?? throw new ArgumentException("Player not found.");

        player.RejectVerification();
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

        Add(
            "Johann Rosas",
            "johann@mail.com",
            DominantHand.Right,
            PreferredSide.Backhand,
            6);

        Add(
            "Franco Banchero",
            "franco@mail.com",
            DominantHand.Right,
            PreferredSide.Drive,
            6);

        Add(
            "Bruno Banchero",
            "bruno@mail.com",
            DominantHand.Left,
            PreferredSide.Drive,
            5);

        Add(
            "Joshua Rosas",
            "joshua@mail.com",
            DominantHand.Right,
            PreferredSide.Both,
            5);
    }

    private bool EmailAlreadyExists(string email)
    {
        return playerRepository.GetAll().Any(player =>
            string.Equals(player.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private bool EmailAlreadyExistsExceptId(string email, Guid id)
    {
        return playerRepository.GetAll().Any(player =>
            player.Id != id &&
            string.Equals(player.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static PlayerDto ToDto(Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            Email = player.Email,
            DominantHand = player.DominantHand,
            PreferredSide = player.PreferredSide,
            Category = player.Category,
            VerificationStatus = player.VerificationStatus,
            RankingPoints = player.RankingPoints,
            HasCompleteProfile = player.HasCompleteProfile,
            IsVerified = player.IsVerified
        };
    }
}