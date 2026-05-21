namespace PadelBracket.Domain.Entities;

public class MatchSet
{
    public int PairOneScore { get; private set; }
    public int PairTwoScore { get; private set; }
    public bool IsSuperTieBreak { get; private set; }

    public MatchSet(int pairOneScore, int pairTwoScore, bool isSuperTieBreak = false)
    {
        if (pairOneScore < 0 || pairTwoScore < 0)
            throw new ArgumentException("Set scores cannot be negative.");

        if (pairOneScore == pairTwoScore)
            throw new ArgumentException("A set cannot end in a draw.");

        if (isSuperTieBreak)
        {
            var winnerScore = Math.Max(pairOneScore, pairTwoScore);
            var difference = Math.Abs(pairOneScore - pairTwoScore);

            if (winnerScore < 11)
                throw new ArgumentException("A super tie-break must be won with at least 11 points.");

            if (difference < 2)
                throw new ArgumentException("A super tie-break must be won by at least 2 points.");
        }

        PairOneScore = pairOneScore;
        PairTwoScore = pairTwoScore;
        IsSuperTieBreak = isSuperTieBreak;
    }

    public bool PairOneWon => PairOneScore > PairTwoScore;
    public bool PairTwoWon => PairTwoScore > PairOneScore;

    public int PairOneGameCount => IsSuperTieBreak ? 0 : PairOneScore;
    public int PairTwoGameCount => IsSuperTieBreak ? 0 : PairTwoScore;

    public string DisplayScore => IsSuperTieBreak
        ? $"STB {PairOneScore}-{PairTwoScore}"
        : $"{PairOneScore}-{PairTwoScore}";
}