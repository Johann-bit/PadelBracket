namespace PadelBracket.Domain.Entities;

public class KnockoutMatch
{
    public Guid Id { get; private set; }
    public string RoundName { get; private set; }
    public Pair? PairOne { get; private set; }
    public Pair? PairTwo { get; private set; }
    public MatchResult? Result { get; private set; }

    public KnockoutMatch(string roundName, Pair? pairOne, Pair? pairTwo)
    {
        if (string.IsNullOrWhiteSpace(roundName))
            throw new ArgumentException("Round name is required.");

        Id = Guid.NewGuid();
        RoundName = roundName.Trim();
        PairOne = pairOne;
        PairTwo = pairTwo;
    }

    public bool HasPairOne => PairOne is not null;
    public bool HasPairTwo => PairTwo is not null;
    public bool IsReadyToPlay => HasPairOne && HasPairTwo;
    public bool HasResult => Result is not null;

    public Pair? Winner
    {
        get
        {
            if (Result is null || PairOne is null || PairTwo is null)
                return null;

            return Result.PairOneWon ? PairOne : PairTwo;
        }
    }

    public Pair? Loser
    {
        get
        {
            if (Result is null || PairOne is null || PairTwo is null)
                return null;

            return Result.PairOneWon ? PairTwo : PairOne;
        }
    }

    public void RegisterResult(MatchResult result)
    {
        if (!IsReadyToPlay)
            throw new InvalidOperationException("Cannot register a result before both pairs are assigned.");

        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public void AssignPairOne(Pair pair)
    {
        PairOne = pair ?? throw new ArgumentNullException(nameof(pair));
    }

    public void AssignPairTwo(Pair pair)
    {
        PairTwo = pair ?? throw new ArgumentNullException(nameof(pair));
    }
}