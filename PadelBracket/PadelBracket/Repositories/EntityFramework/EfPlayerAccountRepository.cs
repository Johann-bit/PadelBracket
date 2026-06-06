using PadelBracket.Data;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class EfPlayerAccountRepository : IPlayerAccountRepository
{
    private readonly ApplicationDbContext dbContext;

    public EfPlayerAccountRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public PlayerAccount? GetByPlayerId(Guid playerId)
    {
        return dbContext.PlayerAccounts.FirstOrDefault(account => account.PlayerId == playerId);
    }

    public PlayerAccount? GetByEmail(string email)
    {
        string trimmedEmail = email.Trim().ToLower();

        return dbContext.PlayerAccounts.FirstOrDefault(account =>
            account.Email == trimmedEmail);
    }

    public void Add(PlayerAccount account)
    {
        dbContext.PlayerAccounts.Add(account);
        dbContext.SaveChanges();
    }

    public void SaveChanges()
    {
        dbContext.SaveChanges();
    }
}