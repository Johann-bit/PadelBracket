namespace PadelBracket.Domain.Entities;

public class MatchResult
{
    public int PairOneGames { get; private set; }
    public int PairTwoGames { get; private set; }

    public MatchResult(int pairOneGames, int pairTwoGames)
    {
        if (pairOneGames < 0 || pairTwoGames < 0)
            throw new ArgumentException("Games cannot be negative.");

        if (pairOneGames == pairTwoGames)
            throw new ArgumentException("A match cannot end in a draw.");

        PairOneGames = pairOneGames;
        PairTwoGames = pairTwoGames;
    }

    public bool PairOneWon => PairOneGames > PairTwoGames;
    public bool PairTwoWon => PairTwoGames > PairOneGames;

    public int PairOneGameDifference => PairOneGames - PairTwoGames;
    public int PairTwoGameDifference => PairTwoGames - PairOneGames;
}