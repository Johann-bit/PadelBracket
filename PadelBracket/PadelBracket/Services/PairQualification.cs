using PadelBracket.Domain.Entities;

namespace PadelBracket.Services;

public class PairQualification
{
    public Pair Pair { get; private set; }
    public QualificationStatus Status { get; private set; }
    public int CurrentPosition { get; private set; }

    public PairQualification(Pair pair, QualificationStatus status, int currentPosition)
    {
        Pair = pair ?? throw new ArgumentNullException(nameof(pair));
        Status = status;
        CurrentPosition = currentPosition;
    }
}