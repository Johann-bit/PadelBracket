namespace PadelBracket.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Player(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name is required.");

        Id = Guid.NewGuid();
        Name = name.Trim();
    }
}