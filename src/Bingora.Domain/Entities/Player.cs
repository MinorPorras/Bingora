namespace Bingora.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid CardId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime JoinedAt { get; private set; }
    public bool IsActive { get; private set; }

    //Navegación
    public Game Game { get; private set; } = null!;
    public Card Card { get; private set; } = null!;

    private Player()
    {
    }

    public static Player Create(Guid gameId, Guid cardId, string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            throw new ArgumentException("El nombre del jugador es obligatorio.", nameof(playerName));

        return new Player
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            CardId = cardId,
            Name = playerName,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}