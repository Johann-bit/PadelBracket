using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface IOrganizerAccountRepository
{
    OrganizerAccount? GetByOrganizerId(Guid organizerId);
    OrganizerAccount? GetByEmail(string email);
    void Add(OrganizerAccount account);
    void SaveChanges();
}