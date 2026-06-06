namespace PadelBracket.Services;

public class PlayerRankingSummary
{
    public Guid PlayerId { get; set; }
    public int Category { get; set; }
    public int? Position { get; set; }
    public PlayerRankingItem? Ranking { get; set; }

    public bool IsRanked => Position.HasValue && Ranking != null;
}