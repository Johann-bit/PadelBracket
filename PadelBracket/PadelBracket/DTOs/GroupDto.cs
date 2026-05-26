namespace PadelBracket.Domain.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Category { get; set; }
    public string CategoryLabel { get; set; } = string.Empty;
    public int PairCount { get; set; }
    public int MatchCount { get; set; }
}