namespace PadelBracket.Domain.Entities;

public class Tournament
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<Group> Groups { get; private set; }

    public Tournament(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        CreatedAt = DateTime.Now;
        Groups = new List<Group>();
    }

    public void AddGroup(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        if (Groups.Any(g => g.Id == group.Id))
            throw new ArgumentException("This group is already in the tournament.");

        Groups.Add(group);
    }
}