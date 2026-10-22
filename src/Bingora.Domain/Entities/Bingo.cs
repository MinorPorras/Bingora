using Bingora.Domain.ValueObjects;

namespace Bingora.Domain.Entities;

public class Bingo
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int TotalCards { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyList<ColumnRange> ColumnRanges { get; private set; } = [];

    public User Owner { get; private set; } = null!;
    public IReadOnlyList<BingoPattern> BingoPatterns => _bingoPatterns.AsReadOnly();
    private readonly List<BingoPattern> _bingoPatterns = [];
    //public ICollection<Card> Cards { get; private set; } = new List<Card>();
    //public ICollection<Game> Games { get; private set; } = new List<Game>();

    private Bingo()
    {
    }

    private static void Validate(string name, int totalCards, IReadOnlyList<ColumnRange> columnRanges)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("El nombre del bingo es obligatorio.", nameof(name));
        if (totalCards <= 0)
            throw new ArgumentException("El total de cartones debe ser mayor a 0", nameof(totalCards));
        if (columnRanges == null || !columnRanges.Any())
            throw new ArgumentException("Se debe de definir al menos un rango de columnas", nameof(columnRanges));
    }

    public static Bingo Create(string name, string description, int totalCards, Guid ownerId,
        IReadOnlyList<ColumnRange> columnRanges)
    {
        Validate(name, totalCards, columnRanges);
        return new Bingo
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            TotalCards = totalCards,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            ColumnRanges = [.. columnRanges]
        };
    }

    public void UpdateConfiguration(string name, string description, int totalCards,
        IReadOnlyList<ColumnRange> columnRanges)
    {
        Validate(name, totalCards, columnRanges);
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        TotalCards = totalCards;
        ColumnRanges = [.. columnRanges];
    }

    public void AddPattern(Guid patternId, int availablePrizes)
    {
        if (availablePrizes <= 0)
            throw new ArgumentException("La cantidad de premios disponibles no puede ser negativa.",
                nameof(availablePrizes));
        if (_bingoPatterns.Any(x => x.PatternId == patternId))
            throw new InvalidOperationException("Este patrón ya está asociado a este Bingo.");
        var bingoPattern = BingoPattern.Create(Id, patternId, availablePrizes);
        _bingoPatterns.Add(bingoPattern);
    }

    public void UpdatePatternPrizes(Guid patternId, int availablePrizes)
    {
        if (availablePrizes <= 0)
            throw new ArgumentException("La cantidad de premios disponibles no puede ser negativa.",
                nameof(availablePrizes));
        var bingoPattern = _bingoPatterns.FirstOrDefault(x => x.PatternId == patternId) ??
                           throw new InvalidOperationException("No se encontró el patrón especificado");
        bingoPattern.UpdatePrizes(availablePrizes);
    }

    public void RemovePattern(Guid patternId)
    {
        var bingoPattern = _bingoPatterns.FirstOrDefault(x => x.PatternId == patternId) ??
                           throw new InvalidOperationException("No se encontró el patrón especificado");
        _bingoPatterns.Remove(bingoPattern);
    }
}