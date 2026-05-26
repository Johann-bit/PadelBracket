namespace PadelBracket.Domain.DTOs;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public int GroupCount { get; set; }
}