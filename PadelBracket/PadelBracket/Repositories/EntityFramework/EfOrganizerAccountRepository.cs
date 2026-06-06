using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfOrganizerAccountRepository : IOrganizerAccountRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfOrganizerAccountRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OrganizerAccount? GetByOrganizerId(Guid organizerId)
    {
        return dbContext.OrganizerAccounts.FirstOrDefault(account => account.OrganizerId == organizerId);
    }

    public OrganizerAccount? GetByEmail(string email)
    {
        string trimmedEmail = email.Trim().ToLower();

        return dbContext.OrganizerAccounts.FirstOrDefault(account =>
            account.Email == trimmedEmail);
    }

    public void Add(OrganizerAccount account)
    {
        dbContext.OrganizerAccounts.Add(account);
        dbContext.SaveChanges();
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}