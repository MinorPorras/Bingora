namespace Bingora.Domain.Entities;

public class BingoPattern
{
    public Guid BingoId { get; private set; }
    public Guid PatternId { get; private set; }
    public int AvailablePrizes { get; private set; }

    //Navegación
    public Bingo Bingo { get; private set; }
    public Pattern Pattern { get; private set; }

    public BingoPattern()
    {
    }

    public static BingoPattern Create(Guid bingoId, Guid patternId, int availablePrizes)
    {
        if (availablePrizes < 0)
            throw new ArgumentException("La cantidad de premios disponibles no puede ser negativa.",
                nameof(availablePrizes));
        return new BingoPattern
        {
            BingoId = bingoId,
            PatternId = patternId,
            AvailablePrizes = availablePrizes
        };
    }

    public void UpdatePrizes(int newAvailablePrizes)
    {
        if (newAvailablePrizes < 0)
            throw new ArgumentException("La cantidad de premios disponibles no puede ser negativa.",
                nameof(newAvailablePrizes));
        AvailablePrizes = newAvailablePrizes;
    }
}