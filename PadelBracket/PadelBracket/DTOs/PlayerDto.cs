using PadelBracket.Domain.Enums;

namespace PadelBracket.Domain.DTOs;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DominantHand? DominantHand { get; set; }
    public PreferredSide? PreferredSide { get; set; }
    public int? Category { get; set; }
    public PlayerVerificationStatus VerificationStatus { get; set; }
    public int RankingPoints { get; set; }
    public bool HasCompleteProfile { get; set; }
    public bool IsVerified { get; set; }

    public string ShortId => Id.ToString()[..8];
}