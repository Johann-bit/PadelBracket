namespace PadelBracket.Domain.Entities;

public class Pair
{
    public Guid Id { get; private set; }
    public Player PlayerOne { get; private set; }
    public Player PlayerTwo { get; private set; }
    public int? Category { get; private set; }

    public string DisplayName => $"{PlayerOne.Name} / {PlayerTwo.Name}";

    private Pair()
    {
        PlayerOne = null!;
        PlayerTwo = null!;
    }

    public Pair(Player playerOne, Player playerTwo)
    {
        PlayerOne = playerOne ?? throw new ArgumentNullException(nameof(playerOne));
        PlayerTwo = playerTwo ?? throw new ArgumentNullException(nameof(playerTwo));

        if (PlayerOne.Id == PlayerTwo.Id)
            throw new ArgumentException("A pair cannot have the same player twice.");

        Id = Guid.NewGuid();
        Category = CalculateCategory();
    }

    public void RenamePlayers(string playerOneName, string playerTwoName)
    {
        if (string.IsNullOrWhiteSpace(playerOneName))
            throw new ArgumentException("Player one name is required.");

        if (string.IsNullOrWhiteSpace(playerTwoName))
            throw new ArgumentException("Player two name is required.");

        PlayerOne.Rename(playerOneName);
        PlayerTwo.Rename(playerTwoName);
    }

    public bool ContainsPlayer(Guid playerId)
    {
        return PlayerOne.Id == playerId || PlayerTwo.Id == playerId;
    }

    public bool HasSamePlayersAs(Pair other)
    {
        if (other == null)
            return false;

        return ContainsPlayer(other.PlayerOne.Id) && ContainsPlayer(other.PlayerTwo.Id);
    }

    public bool IsFullyVerified()
    {
        return PlayerOne.IsVerified && PlayerTwo.IsVerified;
    }

    public bool CanPlayInCategory(int tournamentCategory)
    {
        if (tournamentCategory < 1 || tournamentCategory > 8)
            throw new ArgumentException("Category must be between 1 and 8.");

        if (!Category.HasValue)
            return false;

        return Category.Value <= tournamentCategory;
    }

    public void RefreshCategory()
    {
        Category = CalculateCategory();
    }

    private int? CalculateCategory()
    {
        if (!PlayerOne.Category.HasValue || !PlayerTwo.Category.HasValue)
            return null;

        return Math.Min(PlayerOne.Category.Value, PlayerTwo.Category.Value);
    }
}