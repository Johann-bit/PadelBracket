namespace PadelBracket.Domain.DTOs;

public class OrganizerDto
{
    public Guid Id { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool HasCompleteProfile { get; set; }

    public string ShortId => Id.ToString()[..8];
}