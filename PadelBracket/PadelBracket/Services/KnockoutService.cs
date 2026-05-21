using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class KnockoutService
{
    private readonly StandingService _standingService;

    public KnockoutService(StandingService standingService)
    {
        _standingService = standingService ?? throw new ArgumentNullException(nameof(standingService));
    }

    public List<KnockoutMatch> GenerateSemifinals(
        Tournament tournament,
        int category,
        int qualifiedPairsPerGroup = 2)
    {
        if (tournament == null)
            throw new ArgumentNullException(nameof(tournament));

        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");

        if (qualifiedPairsPerGroup != 2)
            throw new ArgumentException("This knockout version only supports 2 qualified pairs per group.");

        var categoryGroups = tournament.Groups
            .Where(group => group.Category == category)
            .OrderBy(group => group.Name)
            .ToList();

        if (categoryGroups.Count != 2)
            throw new InvalidOperationException("Exactly 2 groups are required to generate semifinals.");

        foreach (var group in categoryGroups)
        {
            ValidateGroupIsReady(group, qualifiedPairsPerGroup);
        }

        var groupOne = categoryGroups[0];
        var groupTwo = categoryGroups[1];

        var groupOneStandings = _standingService.CalculateStandings(groupOne);
        var groupTwoStandings = _standingService.CalculateStandings(groupTwo);

        var groupOneFirst = groupOneStandings[0].Pair;
        var groupOneSecond = groupOneStandings[1].Pair;

        var groupTwoFirst = groupTwoStandings[0].Pair;
        var groupTwoSecond = groupTwoStandings[1].Pair;

        return new List<KnockoutMatch>
        {
            new KnockoutMatch("Semifinal 1", groupOneFirst, groupTwoSecond),
            new KnockoutMatch("Semifinal 2", groupTwoFirst, groupOneSecond)
        };
    }

    private static void ValidateGroupIsReady(Group group, int qualifiedPairsPerGroup)
    {
        if (group.Pairs.Count < qualifiedPairsPerGroup)
            throw new InvalidOperationException("Each group must have enough pairs to generate semifinals.");

        if (!group.Matches.Any())
            throw new InvalidOperationException("All groups must have generated matches.");

        if (group.Matches.Any(match => !match.HasResult))
            throw new InvalidOperationException("All group matches must have results before generating semifinals.");
    }
}