namespace PadelBracket.Domain.Entities;

public class Match
{
    public Guid Id { get; private set; }
    public Pair PairOne { get; private set; }
    public Pair PairTwo { get; private set; }
    public MatchResult? Result { get; private set; }

    public Match(Pair pairOne, Pair pairTwo)
    {
        PairOne = pairOne ?? throw new ArgumentNullException(nameof(pairOne));
        PairTwo = pairTwo ?? throw new ArgumentNullException(nameof(pairTwo));

        if (PairOne.Id == PairTwo.Id)
            throw new ArgumentException("A match cannot have the same pair twice.");

        Id = Guid.NewGuid();
    }

    public void RegisterResult(MatchResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public bool HasResult => Result is not null;

    public Pair? Winner
    {
        get
        {
            if (Result is null)
                return null;

            return Result.PairOneWon ? PairOne : PairTwo;
        }
    }

    public Pair? Loser
    {
        get
        {
            if (Result is null)
                return null;

            return Result.PairOneWon ? PairTwo : PairOne;
        }
    }
}