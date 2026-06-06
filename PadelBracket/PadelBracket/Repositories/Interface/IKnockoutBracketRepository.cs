using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface IKnockoutBracketRepository
{
    List<KnockoutBracket> GetAll();
    List<KnockoutBracket> GetByTournamentId(Guid tournamentId);
    KnockoutBracket? GetByTournamentAndCategory(Guid tournamentId, int category);
    void Add(KnockoutBracket bracket);
    void Delete(KnockoutBracket bracket);
    void DeleteByTournamentId(Guid tournamentId);
    void SaveChanges();
}