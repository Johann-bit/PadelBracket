using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class MatchHistoryItem
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string CategoryLabel { get; set; } = string.Empty;
    public string PairOneName { get; set; } = string.Empty;
    public string PairTwoName { get; set; } = string.Empty;
    public string WinnerName { get; set; } = string.Empty;
    public string LoserName { get; set; } = string.Empty;
    public string Score { get; set; } = string.Empty;
    public DateTime TournamentCreatedAt { get; set; }
}