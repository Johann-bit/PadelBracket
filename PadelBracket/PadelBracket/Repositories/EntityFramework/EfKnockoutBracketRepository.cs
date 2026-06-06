using Microsoft.EntityFrameworkCore;
using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfKnockoutBracketRepository : IKnockoutBracketRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfKnockoutBracketRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public List<KnockoutBracket> GetAll()
    {
        return Query()
            .OrderByDescending(bracket => bracket.CreatedAt)
            .ToList();
    }

    public List<KnockoutBracket> GetByTournamentId(Guid tournamentId)
    {
        return Query()
            .Where(bracket => bracket.TournamentId == tournamentId)
            .OrderBy(bracket => bracket.Category)
            .ToList();
    }

    public KnockoutBracket? GetByTournamentAndCategory(Guid tournamentId, int category)
    {
        return Query().FirstOrDefault(bracket =>
            bracket.TournamentId == tournamentId &&
            bracket.Category == category);
    }

    public void Add(KnockoutBracket bracket)
    {
        dbContext.KnockoutBrackets.Add(bracket);
        dbContext.SaveChanges();
    }

    public void Delete(KnockoutBracket bracket)
    {
        dbContext.KnockoutBrackets.Remove(bracket);
        dbContext.SaveChanges();
    }

    public void DeleteByTournamentId(Guid tournamentId)
    {
        var brackets = Query()
            .Where(bracket => bracket.TournamentId == tournamentId)
            .ToList();

        dbContext.KnockoutBrackets.RemoveRange(brackets);
        dbContext.SaveChanges();
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }

    private IQueryable<KnockoutBracket> Query()
    {
        return dbContext.KnockoutBrackets
            .AsSplitQuery()
            .Include(bracket => bracket.Matches.OrderBy(match => match.SortOrder))
                .ThenInclude(match => match.PairOne)
                    .ThenInclude(pair => pair!.PlayerOne)
            .Include(bracket => bracket.Matches.OrderBy(match => match.SortOrder))
                .ThenInclude(match => match.PairOne)
                    .ThenInclude(pair => pair!.PlayerTwo)
            .Include(bracket => bracket.Matches.OrderBy(match => match.SortOrder))
                .ThenInclude(match => match.PairTwo)
                    .ThenInclude(pair => pair!.PlayerOne)
            .Include(bracket => bracket.Matches.OrderBy(match => match.SortOrder))
                .ThenInclude(match => match.PairTwo)
                    .ThenInclude(pair => pair!.PlayerTwo);
    }
}