using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryKnockoutBracketRepository : IKnockoutBracketRepository
{
    private readonly List<KnockoutBracket> brackets = new();

    public List<KnockoutBracket> GetAll()
    {
        return brackets.ToList();
    }

    public List<KnockoutBracket> GetByTournamentId(Guid tournamentId)
    {
        return brackets
            .Where(bracket => bracket.TournamentId == tournamentId)
            .ToList();
    }

    public KnockoutBracket? GetByTournamentAndCategory(Guid tournamentId, int category)
    {
        return brackets.FirstOrDefault(bracket =>
            bracket.TournamentId == tournamentId &&
            bracket.Category == category);
    }

    public void Add(KnockoutBracket bracket)
    {
        brackets.Add(bracket);
    }

    public void Delete(KnockoutBracket bracket)
    {
        brackets.Remove(bracket);
    }

    public void DeleteByTournamentId(Guid tournamentId)
    {
        brackets.RemoveAll(bracket => bracket.TournamentId == tournamentId);
    }

    public void SaveChanges()
    {
    }
}