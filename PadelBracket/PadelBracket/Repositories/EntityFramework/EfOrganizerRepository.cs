using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfOrganizerRepository : IOrganizerRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfOrganizerRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyList<Organizer> GetAll()
    {
        return dbContext.Organizers
            .OrderBy(organizer => organizer.RealName)
            .ToList();
    }

    public Organizer? GetById(Guid id)
    {
        return dbContext.Organizers.FirstOrDefault(organizer => organizer.Id == id);
    }

    public void Add(Organizer organizer)
    {
        dbContext.Organizers.Add(organizer);
        dbContext.SaveChanges();
    }

    public void Delete(Organizer organizer)
    {
        dbContext.Organizers.Remove(organizer);
        dbContext.SaveChanges();
    }

    public bool ExistsByEmail(string email)
    {
        string trimmedEmail = email.Trim().ToLower();

        return dbContext.Organizers.Any(organizer =>
            organizer.Email == trimmedEmail);
    }

    public bool ExistsByEmailExceptId(string email, Guid id)
    {
        string trimmedEmail = email.Trim().ToLower();

        return dbContext.Organizers.Any(organizer =>
            organizer.Id != id &&
            organizer.Email == trimmedEmail);
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}