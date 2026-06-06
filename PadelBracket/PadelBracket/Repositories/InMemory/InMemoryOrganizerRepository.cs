using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryOrganizerRepository : IOrganizerRepository
{
    private readonly List<Organizer> organizers = new();

    public IReadOnlyList<Organizer> GetAll()
    {
        return organizers;
    }

    public Organizer? GetById(Guid id)
    {
        return organizers.FirstOrDefault(organizer => organizer.Id == id);
    }

    public void Add(Organizer organizer)
    {
        organizers.Add(organizer);
    }

    public void Delete(Organizer organizer)
    {
        organizers.Remove(organizer);
    }

    public bool ExistsByEmail(string email)
    {
        return organizers.Any(organizer =>
            string.Equals(organizer.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool ExistsByEmailExceptId(string email, Guid id)
    {
        return organizers.Any(organizer =>
            organizer.Id != id &&
            string.Equals(organizer.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void SaveChanges()
    {
    }
}