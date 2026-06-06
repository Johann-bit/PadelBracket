using Microsoft.EntityFrameworkCore;
using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfTournamentRepository : ITournamentRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfTournamentRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public List<Tournament> GetAll()
    {
        return dbContext.Tournaments
            .AsSplitQuery()
            .Include(tournament => tournament.TournamentCategories)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Pairs)
                    .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Pairs)
                    .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairOne)
                        .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairOne)
                        .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairTwo)
                        .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairTwo)
                        .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Registrations)
                .ThenInclude(registration => registration.Pair)
                    .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Registrations)
                .ThenInclude(registration => registration.Pair)
                    .ThenInclude(pair => pair.PlayerTwo)
            .OrderByDescending(tournament => tournament.CreatedAt)
            .ToList();
    }

    public Tournament? GetById(Guid id)
    {
        return dbContext.Tournaments
            .AsSplitQuery()
            .Include(tournament => tournament.TournamentCategories)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Pairs)
                    .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Pairs)
                    .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairOne)
                        .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairOne)
                        .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairTwo)
                        .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Groups)
                .ThenInclude(group => group.Matches)
                    .ThenInclude(match => match.PairTwo)
                        .ThenInclude(pair => pair.PlayerTwo)
            .Include(tournament => tournament.Registrations)
                .ThenInclude(registration => registration.Pair)
                    .ThenInclude(pair => pair.PlayerOne)
            .Include(tournament => tournament.Registrations)
                .ThenInclude(registration => registration.Pair)
                    .ThenInclude(pair => pair.PlayerTwo)
            .FirstOrDefault(tournament => tournament.Id == id);
    }

    public void Add(Tournament tournament)
    {
        dbContext.Tournaments.Add(tournament);
        dbContext.SaveChanges();
    }

    public void Delete(Tournament tournament)
    {
        dbContext.Tournaments.Remove(tournament);
        dbContext.SaveChanges();
    }

    public bool ExistsByName(string name)
    {
        string trimmedName = name.Trim().ToLower();

        return dbContext.Tournaments.Any(tournament =>
            tournament.Name.ToLower() == trimmedName);
    }

    public bool ExistsByNameExceptId(string name, Guid id)
    {
        string trimmedName = name.Trim().ToLower();

        return dbContext.Tournaments.Any(tournament =>
            tournament.Id != id &&
            tournament.Name.ToLower() == trimmedName);
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}