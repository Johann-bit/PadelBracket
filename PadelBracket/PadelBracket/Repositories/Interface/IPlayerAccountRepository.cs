using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface IPlayerAccountRepository
{
    PlayerAccount? GetByPlayerId(Guid playerId);
    PlayerAccount? GetByEmail(string email);
    void Add(PlayerAccount account);
    void SaveChanges();
}