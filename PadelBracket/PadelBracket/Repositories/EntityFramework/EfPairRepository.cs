using Microsoft.EntityFrameworkCore;
using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfPairRepository : IPairRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfPairRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyList<Pair> GetAll()
    {
        return dbContext.Pairs
            .AsNoTracking()
            .Include(pair => pair.PlayerOne)
            .Include(pair => pair.PlayerTwo)
            .OrderBy(pair => pair.PlayerOne.Name)
            .ThenBy(pair => pair.PlayerTwo.Name)
            .ToList();
    }

    public Pair? GetById(Guid id)
    {
        return dbContext.Pairs
            .Include(pair => pair.PlayerOne)
            .Include(pair => pair.PlayerTwo)
            .FirstOrDefault(pair => pair.Id == id);
    }

    public void Add(Pair pair)
    {
        dbContext.Pairs.Add(pair);
        dbContext.SaveChanges();
    }

    public void Delete(Pair pair)
    {
        dbContext.Pairs.Remove(pair);
        dbContext.SaveChanges();
    }

    public bool ExistsByPlayers(Guid playerOneId, Guid playerTwoId)
    {
        return dbContext.Pairs.Any(pair =>
            (
                EF.Property<Guid>(pair, "PlayerOneId") == playerOneId &&
                EF.Property<Guid>(pair, "PlayerTwoId") == playerTwoId
            ) ||
            (
                EF.Property<Guid>(pair, "PlayerOneId") == playerTwoId &&
                EF.Property<Guid>(pair, "PlayerTwoId") == playerOneId
            ));
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}
