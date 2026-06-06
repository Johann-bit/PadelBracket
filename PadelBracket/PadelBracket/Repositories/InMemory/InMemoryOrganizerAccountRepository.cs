using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryOrganizerAccountRepository : IOrganizerAccountRepository
{
    private readonly List<OrganizerAccount> accounts = new();

    public OrganizerAccount? GetByOrganizerId(Guid organizerId)
    {
        return accounts.FirstOrDefault(account => account.OrganizerId == organizerId);
    }

    public OrganizerAccount? GetByEmail(string email)
    {
        return accounts.FirstOrDefault(account =>
            string.Equals(account.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void Add(OrganizerAccount account)
    {
        accounts.Add(account);
    }

    public void SaveChanges()
    {
    }
}