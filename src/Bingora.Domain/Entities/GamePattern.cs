namespace Bingora.Domain.Entities;

public class GamePattern
{
    public Guid GameId { get; private set; }
    public Guid PatternId { get; private set; }
    public int TotalPrizes { get; private set; }
    public int ClaimedPrizes { get; private set; } = 0;
    public bool IsActive => ClaimedPrizes < TotalPrizes;

    public Game Game { get; private set; }
    public Pattern Pattern { get; private set; }

    private GamePattern()
    {
    }

    public static GamePattern Create(Guid gameId, Guid patternId, int totalPrizes)
    {
        if (totalPrizes < 0)
            throw new ArgumentException("El total de premios no puede ser negativo.");
        return new GamePattern
        {
            GameId = gameId,
            PatternId = patternId,
            TotalPrizes = totalPrizes,
            ClaimedPrizes = 0
        };
    }

    public void ClaimPrize()
    {
        if (!IsActive)
            throw new InvalidOperationException(
                $"No se pueden reclamar más premios. El límite de {TotalPrizes} premios ya fue alcanzado.");
        ClaimedPrizes++;
    }
}