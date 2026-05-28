using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Domain.Repositories;

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
        if (playerOne.Id == playerTwo.Id)
            throw new ArgumentException("A pair cannot have the same player twice.");

        if (pairRepository.ExistsByPlayers(playerOne.Id, playerTwo.Id))
            throw new ArgumentException("That pair already exists.");

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