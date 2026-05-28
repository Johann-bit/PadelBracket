using PadelBracket.Domain.Enums;

namespace PadelBracket.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DominantHand? DominantHand { get; private set; }
    public PreferredSide? PreferredSide { get; private set; }
    public int? Category { get; private set; }
    public PlayerVerificationStatus VerificationStatus { get; private set; }
    public int RankingPoints { get; private set; }

    public bool IsVerified => VerificationStatus == PlayerVerificationStatus.Verified;

    public bool HasCompleteProfile =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Email) &&
        DominantHand.HasValue &&
        PreferredSide.HasValue &&
        Category.HasValue;

    public Player(string name)
    {
        ValidateName(name);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = string.Empty;
        DominantHand = null;
        PreferredSide = null;
        Category = null;
        VerificationStatus = PlayerVerificationStatus.Pending;
        RankingPoints = 0;
    }

    public Player(
        string name,
        string email,
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidateCategory(category);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim().ToLower();
        DominantHand = dominantHand;
        PreferredSide = preferredSide;
        Category = category;
        VerificationStatus = PlayerVerificationStatus.Pending;
        RankingPoints = GetBaseRankingPoints(category);
    }

    public void Rename(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    public void UpdatePersonalData(string name, string email)
    {
        ValidateName(name);
        ValidateEmail(email);

        Name = name.Trim();
        Email = email.Trim().ToLower();
        VerificationStatus = PlayerVerificationStatus.Pending;
    }

    public void UpdateSportData(
        DominantHand dominantHand,
        PreferredSide preferredSide,
        int category)
    {
        ValidateCategory(category);

        DominantHand = dominantHand;
        PreferredSide = preferredSide;
        Category = category;
        RankingPoints = GetBaseRankingPoints(category);
        VerificationStatus = PlayerVerificationStatus.Pending;
    }

    public void Verify()
    {
        if (!HasCompleteProfile)
            throw new InvalidOperationException("Player profile must be complete before verification.");

        VerificationStatus = PlayerVerificationStatus.Verified;
    }

    public void RejectVerification()
    {
        VerificationStatus = PlayerVerificationStatus.Rejected;
    }

    public void AddRankingPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be greater than zero.");

        RankingPoints += points;
    }

    public void RemoveRankingPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be greater than zero.");

        RankingPoints = Math.Max(0, RankingPoints - points);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name is required.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Player email is required.");

        if (!email.Contains('@'))
            throw new ArgumentException("Player email is invalid.");
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }

    private static int GetBaseRankingPoints(int category)
    {
        return category switch
        {
            1 => 3500,
            2 => 2600,
            3 => 1900,
            4 => 1300,
            5 => 850,
            6 => 500,
            7 => 250,
            8 => 100,
            _ => throw new ArgumentException("Category must be between 1 and 8.")
        };
    }
}