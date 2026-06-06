namespace PadelBracket.Services;

public class RankingItem
{
    public Guid PairId { get; set; }
    public string PairName { get; set; } = string.Empty;
    public int Category { get; set; }
    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GameDifference => GamesWon - GamesLost;
    public double WinRate => MatchesPlayed == 0 ? 0 : (double)Wins / MatchesPlayed * 100;
}