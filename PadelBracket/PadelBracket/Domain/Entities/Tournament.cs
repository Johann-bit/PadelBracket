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

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");

        Name = name.Trim();
    }

    public void AddGroup(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        if (Groups.Any(g => g.Id == group.Id))
            throw new ArgumentException("This group is already in the tournament.");

        if (Groups.Any(g =>
                g.Category == group.Category &&
                string.Equals(g.Name, group.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A group with the same name already exists in this category.");
        }

        Groups.Add(group);
    }

    public void RenameGroup(Guid groupId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.");

        var group = Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        if (Groups.Any(existingGroup =>
                existingGroup.Id != groupId &&
                existingGroup.Category == group.Category &&
                string.Equals(existingGroup.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A group with the same name already exists in this category.");
        }

        group.Rename(name);
    }

    public void RemoveGroup(Guid groupId)
    {
        var group = Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        if (group.Matches.Any())
            throw new InvalidOperationException("Cannot delete a group after matches have been generated.");

        Groups.Remove(group);
    }
}