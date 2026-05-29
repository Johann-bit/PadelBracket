namespace PadelBracket.Domain.Entities;

public class TournamentCategory
{
    public Guid Id { get; private set; }
    public int Category { get; private set; }
    public int MaxPairs { get; private set; }
    public decimal RegistrationFee { get; private set; }

    public TournamentCategory(int category, int maxPairs, decimal registrationFee)
    {
        ValidateCategory(category);
        ValidateMaxPairs(maxPairs);
        ValidateRegistrationFee(registrationFee);

        Id = Guid.NewGuid();
        Category = category;
        MaxPairs = maxPairs;
        RegistrationFee = registrationFee;
    }

    public void UpdateMaxPairs(int maxPairs)
    {
        ValidateMaxPairs(maxPairs);

        MaxPairs = maxPairs;
    }

    public void UpdateRegistrationFee(decimal registrationFee)
    {
        ValidateRegistrationFee(registrationFee);

        RegistrationFee = registrationFee;
    }

    public bool HasAvailableSlot(int currentPairsCount)
    {
        if (currentPairsCount < 0)
            throw new ArgumentException("Current pairs count cannot be negative.");

        return currentPairsCount < MaxPairs;
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }

    private static void ValidateMaxPairs(int maxPairs)
    {
        if (maxPairs <= 0)
            throw new ArgumentException("Max pairs must be greater than zero.");
    }

    private static void ValidateRegistrationFee(decimal registrationFee)
    {
        if (registrationFee < 0)
            throw new ArgumentException("Registration fee cannot be negative.");
    }
}