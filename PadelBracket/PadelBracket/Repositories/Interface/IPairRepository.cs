using PadelBracket.Domain.Entities;

namespace PadelBracket.Repositories.Interface;

public interface IPairRepository
{
    IReadOnlyList<Pair> GetAll();
    Pair? GetById(Guid id);
    void Add(Pair pair);
    void Delete(Pair pair);
    bool ExistsByPlayers(Guid playerOneId, Guid playerTwoId);
}