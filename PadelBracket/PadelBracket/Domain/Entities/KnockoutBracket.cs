namespace PadelBracket.Domain.Entities;

public class KnockoutBracket
{
    public Guid Id { get; private set; }
    public Guid TournamentId { get; private set; }
    public int Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<KnockoutMatch> Matches { get; private set; }

    public KnockoutBracket(Guid tournamentId, int category, List<KnockoutMatch> matches)
    {
        if (tournamentId == Guid.Empty)
            throw new ArgumentException("Tournament id is required.");

        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");

        if (matches == null)
            throw new ArgumentNullException(nameof(matches));

        if (!matches.Any())
            throw new ArgumentException("A knockout bracket must have at least one match.");

        Id = Guid.NewGuid();
        TournamentId = tournamentId;
        Category = category;
        CreatedAt = DateTime.Now;
        Matches = matches;
    }
}