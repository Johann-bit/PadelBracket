using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class PairService
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

    public Pair Add(Player playerOne, Player playerTwo)
    {
        if (playerOne.Id == playerTwo.Id)
            throw new ArgumentException("A pair cannot have the same player twice.");

        bool pairAlreadyExists = pairs.Any(pair =>
            (pair.PlayerOne.Id == playerOne.Id && pair.PlayerTwo.Id == playerTwo.Id) ||
            (pair.PlayerOne.Id == playerTwo.Id && pair.PlayerTwo.Id == playerOne.Id));

        if (pairAlreadyExists)
            throw new ArgumentException("That pair already exists.");

        Pair pair = new Pair(playerOne, playerTwo);

        pairs.Add(pair);

        return pair;
    }

    public void Delete(Guid id)
    {
        Pair pair = GetById(id)
            ?? throw new ArgumentException("Pair not found.");

        pairs.Remove(pair);
    }
}