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
            ValidateSuperTieBreak(pairOneScore, pairTwoScore);
        }
        else
        {
            ValidateRegularSet(pairOneScore, pairTwoScore);
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

    private static void ValidateRegularSet(int pairOneScore, int pairTwoScore)
    {
        var winnerScore = Math.Max(pairOneScore, pairTwoScore);
        var loserScore = Math.Min(pairOneScore, pairTwoScore);

        if (winnerScore > 7)
            throw new ArgumentException("A regular set cannot have more than 7 games for the winner.");

        if (winnerScore < 6)
            throw new ArgumentException("A regular set must be won with 6 or 7 games.");

        var isValidSixGameSet = winnerScore == 6 && loserScore <= 4;
        var isValidSevenFiveSet = winnerScore == 7 && loserScore == 5;
        var isValidSevenSixSet = winnerScore == 7 && loserScore == 6;

        if (!isValidSixGameSet && !isValidSevenFiveSet && !isValidSevenSixSet)
        {
            throw new ArgumentException(
                "Invalid regular set score. Valid examples: 6-0, 6-4, 7-5 or 7-6."
            );
        }
    }

    private static void ValidateSuperTieBreak(int pairOneScore, int pairTwoScore)
    {
        var winnerScore = Math.Max(pairOneScore, pairTwoScore);
        var difference = Math.Abs(pairOneScore - pairTwoScore);

        if (winnerScore < 11)
            throw new ArgumentException("A super tie-break must be won with at least 11 points.");

        if (difference < 2)
            throw new ArgumentException("A super tie-break must be won by at least 2 points.");
    }
}