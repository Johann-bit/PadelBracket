namespace PadelBracket.Domain.Entities;

public class Pair
{
    public Guid Id { get; private set; }
    public Player PlayerOne { get; private set; }
    public Player PlayerTwo { get; private set; }

    public Pair(Player playerOne, Player playerTwo)
    {
        PlayerOne = playerOne ?? throw new ArgumentNullException(nameof(playerOne));
        PlayerTwo = playerTwo ?? throw new ArgumentNullException(nameof(playerTwo));

        if (PlayerOne.Id == PlayerTwo.Id)
            throw new ArgumentException("A pair cannot have the same player twice.");

        Id = Guid.NewGuid();
    }

    public string DisplayName => $"{PlayerOne.Name} / {PlayerTwo.Name}";
}