namespace PadelBracket.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Category { get; private set; }
    public List<Pair> Pairs { get; private set; }
    public List<Match> Matches { get; private set; }

    public string CategoryLabel => GetCategoryLabel(Category);

    public Group(string name, int category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.");

        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Category = category;
        Pairs = new List<Pair>();
        Matches = new List<Match>();
    }

    public void AddPair(Pair pair)
    {
        if (pair == null)
            throw new ArgumentNullException(nameof(pair));

        if (Matches.Any())
            throw new InvalidOperationException("Cannot add pairs after matches have been generated.");

        if (Pairs.Any(p => p.Id == pair.Id))
            throw new ArgumentException("This pair is already in the group.");

        Pairs.Add(pair);
    }

    public void GenerateMatches()
    {
        if (Pairs.Count < 2)
            throw new InvalidOperationException("At least two pairs are required to generate matches.");

        if (Matches.Any())
            throw new InvalidOperationException("Matches have already been generated for this group.");

        for (int i = 0; i < Pairs.Count; i++)
        {
            for (int j = i + 1; j < Pairs.Count; j++)
            {
                Matches.Add(new Match(Pairs[i], Pairs[j]));
            }
        }
    }

    private static string GetCategoryLabel(int category)
    {
        return category switch
        {
            1 => "1ra categoría",
            2 => "2da categoría",
            3 => "3ra categoría",
            4 => "4ta categoría",
            5 => "5ta categoría",
            6 => "6ta categoría",
            7 => "7ma categoría",
            8 => "8va categoría",
            _ => $"{category} categoría"
        };
    }
}