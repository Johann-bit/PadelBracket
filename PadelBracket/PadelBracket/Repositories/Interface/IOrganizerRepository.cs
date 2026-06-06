using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface IOrganizerRepository
{
    IReadOnlyList<Organizer> GetAll();
    Organizer? GetById(Guid id);
    void Add(Organizer organizer);
    void Delete(Organizer organizer);
    bool ExistsByEmail(string email);
    bool ExistsByEmailExceptId(string email, Guid id);
    void SaveChanges();
}