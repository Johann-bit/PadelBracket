using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryPairRepository : IPairRepository
{
    private readonly List<Pair> pairs = new();

    public IReadOnlyList<Pair> GetAll()
    {
        return pairs;
    }

    public Pair? GetById(Guid id)
    {
        return pairs.FirstOrDefault(pair => pair.Id == id);
    }

    public void Add(Pair pair)
    {
        pairs.Add(pair);
    }

    public void Delete(Pair pair)
    {
        pairs.Remove(pair);
    }

    public bool ExistsByPlayers(Guid playerOneId, Guid playerTwoId)
    {
        return pairs.Any(pair =>
            (pair.PlayerOne.Id == playerOneId && pair.PlayerTwo.Id == playerTwoId) ||
            (pair.PlayerOne.Id == playerTwoId && pair.PlayerTwo.Id == playerOneId));
    }

    public void SaveChanges()
    {
    }
}