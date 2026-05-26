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

    public Pair? GetById(Guid id)
    {
        return pairRepository.GetById(id);
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
}