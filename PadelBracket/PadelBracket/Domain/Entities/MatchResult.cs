namespace PadelBracket.Domain.Entities;

public class MatchResult
{
    public List<MatchSet> Sets { get; private set; }

    public MatchResult(List<MatchSet> sets)
    {
        if (sets == null)
            throw new ArgumentNullException(nameof(sets));

        if (sets.Count != 2 && sets.Count != 3)
            throw new ArgumentException("A match must have 2 sets or 2 sets plus a super tie-break.");

        Sets = sets;

        ValidateMatchResult();
    }

    public int PairOneSetsWon => Sets.Count(set => set.PairOneWon);
    public int PairTwoSetsWon => Sets.Count(set => set.PairTwoWon);

    public int PairOneGames => Sets.Sum(set => set.PairOneGameCount);
    public int PairTwoGames => Sets.Sum(set => set.PairTwoGameCount);

    public bool PairOneWon => PairOneSetsWon > PairTwoSetsWon;
    public bool PairTwoWon => PairTwoSetsWon > PairOneSetsWon;

    public string DisplayScore => string.Join(" / ", Sets.Select(set => set.DisplayScore));

    private void ValidateMatchResult()
    {
        if (Sets.Count == 2)
        {
            if (Sets.Any(set => set.IsSuperTieBreak))
                throw new ArgumentException("A 2-set match cannot include a super tie-break.");

            if (PairOneSetsWon != 2 && PairTwoSetsWon != 2)
                throw new ArgumentException("A 2-set match must be won by the same pair in both sets.");
        }

        if (Sets.Count == 3)
        {
            if (!Sets[2].IsSuperTieBreak)
                throw new ArgumentException("The third set must be a super tie-break.");

            var firstTwoSets = Sets.Take(2).ToList();

            var pairOneFirstTwoSetsWon = firstTwoSets.Count(set => set.PairOneWon);
            var pairTwoFirstTwoSetsWon = firstTwoSets.Count(set => set.PairTwoWon);

            if (pairOneFirstTwoSetsWon != 1 || pairTwoFirstTwoSetsWon != 1)
                throw new ArgumentException("A super tie-break is only played when each pair wins one set.");

            if (PairOneSetsWon != 2 && PairTwoSetsWon != 2)
                throw new ArgumentException("A match winner must win 2 sets.");
        }
    }
}