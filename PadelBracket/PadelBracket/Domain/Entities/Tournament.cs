namespace PadelBracket.Domain.Entities;

public class Tournament
{
    public Guid Id { get; private set; }
    public Guid? OrganizerId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public TournamentStatus Status { get; private set; }
    public string ClubName { get; private set; }
    public string City { get; private set; }
    public string Address { get; private set; }
    public DateTime StartDate { get; private set; }
    public List<Group> Groups { get; private set; }
    public List<TournamentCategory> TournamentCategories { get; private set; }
    public List<TournamentRegistration> Registrations { get; private set; }

    public string StatusLabel => GetStatusLabel(Status);

    public Tournament(string name)
        : this(
            name,
            "Club sin definir",
            "Montevideo",
            "Dirección sin definir",
            DateTime.Today)
    {
    }

    public Tournament(string name, Guid organizerId)
        : this(
            name,
            "Club sin definir",
            "Montevideo",
            "Dirección sin definir",
            DateTime.Today,
            organizerId)
    {
    }

    public Tournament(
        string name,
        string clubName,
        string city,
        string address,
        DateTime startDate)
        : this(
            name,
            clubName,
            city,
            address,
            startDate,
            null)
    {
    }

    public Tournament(
        string name,
        string clubName,
        string city,
        string address,
        DateTime startDate,
        Guid organizerId)
        : this(
            name,
            clubName,
            city,
            address,
            startDate,
            (Guid?)organizerId)
    {
    }

    private Tournament(
        string name,
        string clubName,
        string city,
        string address,
        DateTime startDate,
        Guid? organizerId)
    {
        ValidateName(name);
        ValidateRequiredText(clubName, "Club name is required.");
        ValidateRequiredText(city, "City is required.");
        ValidateRequiredText(address, "Address is required.");

        if (organizerId.HasValue && organizerId.Value == Guid.Empty)
            throw new ArgumentException("Organizer id is required.");

        Id = Guid.NewGuid();
        OrganizerId = organizerId;
        Name = name.Trim();
        ClubName = clubName.Trim();
        City = city.Trim();
        Address = address.Trim();
        StartDate = startDate.Date;
        CreatedAt = DateTime.Now;
        Status = TournamentStatus.Draft;
        Groups = new List<Group>();
        TournamentCategories = new List<TournamentCategory>();
        Registrations = new List<TournamentRegistration>();
    }

    public void Rename(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    public void UpdateDetails(
        string clubName,
        string city,
        string address,
        DateTime startDate)
    {
        ValidateRequiredText(clubName, "Club name is required.");
        ValidateRequiredText(city, "City is required.");
        ValidateRequiredText(address, "Address is required.");

        ClubName = clubName.Trim();
        City = city.Trim();
        Address = address.Trim();
        StartDate = startDate.Date;
    }

    public void AddCategory(TournamentCategory tournamentCategory)
    {
        if (tournamentCategory == null)
            throw new ArgumentNullException(nameof(tournamentCategory));

        if (Status != TournamentStatus.Draft)
            throw new InvalidOperationException("Categories can only be added while the tournament is in draft status.");

        if (TournamentCategories.Any(category => category.Category == tournamentCategory.Category))
            throw new ArgumentException("This category already exists in the tournament.");

        TournamentCategories.Add(tournamentCategory);
    }

    public void RemoveCategory(int category)
    {
        ValidateCategory(category);

        if (Status != TournamentStatus.Draft)
            throw new InvalidOperationException("Categories can only be removed while the tournament is in draft status.");

        TournamentCategory? tournamentCategory = TournamentCategories
            .FirstOrDefault(existingCategory => existingCategory.Category == category);

        if (tournamentCategory == null)
            throw new ArgumentException("Tournament category not found.");

        if (Registrations.Any(registration =>
                registration.Category == category &&
                registration.IsActive()))
        {
            throw new InvalidOperationException("Cannot remove a category with active registrations.");
        }

        TournamentCategories.Remove(tournamentCategory);
    }

    public bool HasCategory(int category)
    {
        ValidateCategory(category);

        return TournamentCategories.Any(tournamentCategory => tournamentCategory.Category == category);
    }

    public TournamentCategory GetCategory(int category)
    {
        ValidateCategory(category);

        return TournamentCategories.FirstOrDefault(tournamentCategory => tournamentCategory.Category == category)
            ?? throw new ArgumentException("Tournament category not found.");
    }

    public void AddRegistration(TournamentRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        if (registration.TournamentId != Id)
            throw new ArgumentException("Registration does not belong to this tournament.");

        if (!HasCategory(registration.Category))
            throw new ArgumentException("Tournament does not contain this category.");

        if (Registrations.Any(existingRegistration =>
                existingRegistration.Pair.HasSamePlayersAs(registration.Pair) &&
                existingRegistration.IsActive()))
        {
            throw new ArgumentException("This pair is already registered in the tournament.");
        }

        if (Registrations.Any(existingRegistration =>
                existingRegistration.IsActive() &&
                (
                    existingRegistration.Pair.ContainsPlayer(registration.Pair.PlayerOne.Id) ||
                    existingRegistration.Pair.ContainsPlayer(registration.Pair.PlayerTwo.Id)
                )))
        {
            throw new ArgumentException("One of the players is already registered in this tournament.");
        }

        int activeRegistrationsInCategory = GetActiveRegistrationsByCategory(registration.Category).Count;

        TournamentCategory tournamentCategory = GetCategory(registration.Category);

        if (!tournamentCategory.HasAvailableSlot(activeRegistrationsInCategory))
            throw new InvalidOperationException("There are no available slots in this category.");

        Registrations.Add(registration);
    }

    public IReadOnlyList<TournamentRegistration> GetActiveRegistrationsByCategory(int category)
    {
        ValidateCategory(category);

        return Registrations
            .Where(registration =>
                registration.Category == category &&
                registration.IsActive())
            .ToList();
    }

    public void AddGroup(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        if (Groups.Any(g => g.Id == group.Id))
            throw new ArgumentException("This group is already in the tournament.");

        if (Groups.Any(g =>
                g.Category == group.Category &&
                string.Equals(g.Name, group.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A group with the same name already exists in this category.");
        }

        Groups.Add(group);
    }

    public void RenameGroup(Guid groupId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.");

        Group? group = Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        if (Groups.Any(existingGroup =>
                existingGroup.Id != groupId &&
                existingGroup.Category == group.Category &&
                string.Equals(existingGroup.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A group with the same name already exists in this category.");
        }

        group.Rename(name);
    }

    public void RemoveGroup(Guid groupId)
    {
        Group? group = Groups.FirstOrDefault(group => group.Id == groupId);

        if (group == null)
            throw new ArgumentException("Group not found.");

        if (group.Matches.Any())
            throw new InvalidOperationException("Cannot delete a group after matches have been generated.");

        Groups.Remove(group);
    }

    public void StartGroupStage()
    {
        if (Status == TournamentStatus.Cancelled)
            throw new InvalidOperationException("A cancelled tournament cannot be started.");

        if (Status == TournamentStatus.Finished)
            throw new InvalidOperationException("A finished tournament cannot be modified.");

        if (Status == TournamentStatus.Draft)
            Status = TournamentStatus.GroupStageInProgress;
    }

    public void StartKnockoutStage()
    {
        if (Status == TournamentStatus.Cancelled)
            throw new InvalidOperationException("A cancelled tournament cannot start knockout stage.");

        if (Status == TournamentStatus.Finished)
            throw new InvalidOperationException("A finished tournament cannot be modified.");

        if (Status == TournamentStatus.Draft || Status == TournamentStatus.GroupStageInProgress)
            Status = TournamentStatus.KnockoutInProgress;
    }

    public void Finish()
    {
        if (Status == TournamentStatus.Cancelled)
            throw new InvalidOperationException("A cancelled tournament cannot be finished.");

        if (Status == TournamentStatus.Draft)
            throw new InvalidOperationException("A draft tournament cannot be finished.");

        Status = TournamentStatus.Finished;
    }

    public void Cancel()
    {
        if (Status == TournamentStatus.Finished)
            throw new InvalidOperationException("A finished tournament cannot be cancelled.");

        Status = TournamentStatus.Cancelled;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tournament name is required.");
    }

    private static void ValidateRequiredText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(message);
    }

    private static void ValidateCategory(int category)
    {
        if (category < 1 || category > 8)
            throw new ArgumentException("Category must be between 1 and 8.");
    }

    private static string GetStatusLabel(TournamentStatus status)
    {
        return status switch
        {
            TournamentStatus.Draft => "Borrador",
            TournamentStatus.GroupStageInProgress => "Fase de grupos",
            TournamentStatus.KnockoutInProgress => "Eliminatoria",
            TournamentStatus.Finished => "Finalizado",
            TournamentStatus.Cancelled => "Cancelado",
            _ => status.ToString()
        };
    }
}