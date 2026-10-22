namespace Bingora.Domain.Entities;

public class DrawnValue
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public int DrawOrder { get; private set; }
    public DateTime DrawnAt { get; private set; }

    public Game Game { get; private set; } = null!;

    private DrawnValue()
    {
    }

    internal DrawnValue(Guid gameId, string value, int drawOrder)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El valor sacado no puede estar vacío.", nameof(value));
        if (drawOrder < 1)
            throw new ArgumentException("El orden de sorteo debe ser mayor a cero.", nameof(drawOrder));

        Id = Guid.NewGuid();
        GameId = gameId;
        Value = value;
        DrawOrder = drawOrder;
        DrawnAt = DateTime.UtcNow;
    }
}