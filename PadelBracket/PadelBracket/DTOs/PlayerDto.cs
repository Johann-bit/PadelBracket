namespace PadelBracket.Domain.DTOs;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string ShortId => Id.ToString()[..8];
}