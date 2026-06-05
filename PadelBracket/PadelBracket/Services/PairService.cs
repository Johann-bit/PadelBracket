using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Services;

public class PairService
{
    private readonly IPairRepository pairRepository;

    public PairService(IPairRepository pairRepository)
    {
        this.pairRepository = pairRepository;
    }

    public IReadOnlyList<Pair> GetAll()
    {
        return pairRepository.GetAll();
    }

    public IReadOnlyList<PairDto> GetAllDtos()
    {
        return pairRepository.GetAll()
            .Select(ToDto)
            .ToList();
    }

    public IReadOnlyList<PairDto> GetDtosByPlayerId(Guid playerId)
    {
        return pairRepository.GetAll()
            .Where(pair => pair.ContainsPlayer(playerId))
            .Select(ToDto)
            .ToList();
    }

    public bool PlayerHasPair(Guid playerId)
    {
        return pairRepository.GetAll()
            .Any(pair => pair.ContainsPlayer(playerId));
    }

    public Pair? GetById(Guid id)
    {
        return pairRepository.GetById(id);
    }

    public PairDto? GetDtoById(Guid id)
    {
        Pair? pair = GetById(id);

        if (pair == null)
            return null;

        return ToDto(pair);
    }

    public Pair Add(Player playerOne, Player playerTwo)
    {
        if (playerOne == null)
            throw new ArgumentNullException(nameof(playerOne));

        if (playerTwo == null)
            throw new ArgumentNullException(nameof(playerTwo));

        if (playerOne.Id == playerTwo.Id)
            throw new ArgumentException("A pair cannot have the same player twice.");

        if (!playerOne.HasCompleteProfile)
            throw new InvalidOperationException("Player one must have a complete profile before creating a pair.");

        if (!playerTwo.HasCompleteProfile)
            throw new InvalidOperationException("Player two must have a complete profile before creating a pair.");

        if (pairRepository.ExistsByPlayers(playerOne.Id, playerTwo.Id))
            throw new ArgumentException("That pair already exists.");

        if (PlayerHasPair(playerOne.Id))
            throw new InvalidOperationException("Player one already has an active pair.");

        if (PlayerHasPair(playerTwo.Id))
            throw new InvalidOperationException("Player two already has an active pair.");

        Pair pair = new Pair(playerOne, playerTwo);

        pairRepository.Add(pair);

        return pair;
    }

    public void Delete(Guid id)
    {
        Pair pair = GetById(id)
            ?? throw new ArgumentException("Pair not found.");

        pairRepository.Delete(pair);
    }

    private static PairDto ToDto(Pair pair)
    {
        return new PairDto
        {
            Id = pair.Id,
            PlayerOneId = pair.PlayerOne.Id,
            PlayerOneName = pair.PlayerOne.Name,
            PlayerTwoId = pair.PlayerTwo.Id,
            PlayerTwoName = pair.PlayerTwo.Name,
            Category = pair.Category,
            IsFullyVerified = pair.IsFullyVerified()
        };
    }
}