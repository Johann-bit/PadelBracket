namespace PadelBracket.Domain.Entities;

public class Organizer
{
    public Guid Id { get; private set; }
    public string RealName { get; private set; }
    public string Email { get; private set; }
    public string ClubName { get; private set; }
    public string City { get; private set; }
    public string Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool HasCompleteProfile =>
        !string.IsNullOrWhiteSpace(RealName) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(ClubName) &&
        !string.IsNullOrWhiteSpace(City) &&
        !string.IsNullOrWhiteSpace(Phone);

    public Organizer(
        string realName,
        string email,
        string clubName,
        string city,
        string phone)
    {
        ValidateRequired(realName, "Organizer name is required.");
        ValidateEmail(email);
        ValidateRequired(clubName, "Club name is required.");
        ValidateRequired(city, "City is required.");
        ValidateRequired(phone, "Phone is required.");

        Id = Guid.NewGuid();
        RealName = realName.Trim();
        Email = email.Trim().ToLower();
        ClubName = clubName.Trim();
        City = city.Trim();
        Phone = phone.Trim();
        CreatedAt = DateTime.Now;
    }

    public void UpdateProfile(
        string realName,
        string email,
        string clubName,
        string city,
        string phone)
    {
        ValidateRequired(realName, "Organizer name is required.");
        ValidateEmail(email);
        ValidateRequired(clubName, "Club name is required.");
        ValidateRequired(city, "City is required.");
        ValidateRequired(phone, "Phone is required.");

        RealName = realName.Trim();
        Email = email.Trim().ToLower();
        ClubName = clubName.Trim();
        City = city.Trim();
        Phone = phone.Trim();
    }

    private static void ValidateRequired(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(message);
    }

    private static void ValidateEmail(string email)
    {
        ValidateRequired(email, "Organizer email is required.");

        if (!email.Contains('@'))
            throw new ArgumentException("Organizer email is invalid.");
    }
}