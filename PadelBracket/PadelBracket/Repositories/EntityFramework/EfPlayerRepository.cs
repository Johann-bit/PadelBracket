using Microsoft.EntityFrameworkCore;
using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfPlayerRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyList<Player> GetAll()
    {
        return dbContext.Players
            .AsNoTracking()
            .OrderBy(player => player.Name)
            .ToList();
    }

    public Player? GetById(Guid id)
    {
        return dbContext.Players.FirstOrDefault(player => player.Id == id);
    }

    public void Add(Player player)
    {
        dbContext.Players.Add(player);
        dbContext.SaveChanges();
    }

    public void Delete(Player player)
    {
        dbContext.Players.Remove(player);
        dbContext.SaveChanges();
    }

    public bool ExistsByName(string name)
    {
        string trimmedName = name.Trim();

        return dbContext.Players.Any(player =>
            player.Name.ToLower() == trimmedName.ToLower());
    }

    public bool ExistsByNameExceptId(string name, Guid id)
    {
        string trimmedName = name.Trim();

        return dbContext.Players.Any(player =>
            player.Id != id &&
            player.Name.ToLower() == trimmedName.ToLower());
    }

    public bool HasAny()
    {
        return dbContext.Players.Any();
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}
