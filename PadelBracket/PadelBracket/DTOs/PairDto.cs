namespace PadelBracket.Domain.DTOs;

public class PairDto
{
    public Guid Id { get; set; }
    public Guid PlayerOneId { get; set; }
    public string PlayerOneName { get; set; } = string.Empty;
    public Guid PlayerTwoId { get; set; }
    public string PlayerTwoName { get; set; } = string.Empty;
    public int? Category { get; set; }
    public bool IsFullyVerified { get; set; }

    public string DisplayName => $"{PlayerOneName} / {PlayerTwoName}";
}