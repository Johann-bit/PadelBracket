namespace PadelBracket.Domain.Entities;

public class Standing
{
    public Pair Pair { get; private set; }

    public int Played { get; private set; }
    public int Won { get; private set; }
    public int Lost { get; private set; }

    public int GamesFor { get; private set; }
    public int GamesAgainst { get; private set; }

    public int GameDifference => GamesFor - GamesAgainst;
    public int Points => Won * 3;

    public Standing(Pair pair)
    {
        Pair = pair ?? throw new ArgumentNullException(nameof(pair));
    }

    public void AddWin(int gamesFor, int gamesAgainst)
    {
        Played++;
        Won++;
        GamesFor += gamesFor;
        GamesAgainst += gamesAgainst;
    }

    public void AddLoss(int gamesFor, int gamesAgainst)
    {
        Played++;
        Lost++;
        GamesFor += gamesFor;
        GamesAgainst += gamesAgainst;
    }
}