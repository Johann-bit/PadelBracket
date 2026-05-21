namespace PadelBracket.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public List<Pair> Pairs { get; private set; }
    public List<Match> Matches { get; private set; }

    public Group(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Pairs = new List<Pair>();
        Matches = new List<Match>();
    }

    public void AddPair(Pair pair)
    {
        if (pair == null)
            throw new ArgumentNullException(nameof(pair));

        if (Pairs.Any(p => p.Id == pair.Id))
            throw new ArgumentException("This pair is already in the group.");

        Pairs.Add(pair);
    }

    public void GenerateMatches()
    {
        Matches.Clear();

        for (int i = 0; i < Pairs.Count; i++)
        {
            for (int j = i + 1; j < Pairs.Count; j++)
            {
                Matches.Add(new Match(Pairs[i], Pairs[j]));
            }
        }
    }
}