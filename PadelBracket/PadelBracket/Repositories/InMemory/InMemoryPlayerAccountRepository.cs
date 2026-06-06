using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Repositories;

public class InMemoryPlayerAccountRepository : IPlayerAccountRepository
{
    private readonly List<PlayerAccount> accounts = new();

    public PlayerAccount? GetByPlayerId(Guid playerId)
    {
        return accounts.FirstOrDefault(account => account.PlayerId == playerId);
    }

    public PlayerAccount? GetByEmail(string email)
    {
        return accounts.FirstOrDefault(account =>
            string.Equals(account.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void Add(PlayerAccount account)
    {
        accounts.Add(account);
    }

    public void SaveChanges()
    {
    }
}