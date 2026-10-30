namespace Bingora.Domain.Entities;

public class Card
{
    public Guid Id { get; private set; }
    public Guid BingoId { get; private set; }
    public Guid GameId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public Guid? PlayerId { get; private set; }
    public string QrCodePath { get; private set; } = string.Empty;
    public string Values { get; private set; } = string.Empty; //Ej: "1,15,22..."
    public int Bitmask { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;


    //Navegación
    public Bingo Bingo { get; private set; }
    public Game Game { get; private set; }
    public Player? Player { get; private set; }

    private Card()
    {
    }

    public void SoftDelete()
    {
        if (!IsActive)
            throw new InvalidOperationException("El cartón ya está eliminado.");
        IsActive = false;
    }

    public void Restore()
    {
        if (IsActive)
            throw new InvalidOperationException("El cartón ya está activo.");
        IsActive = true;
    }

    public static Card Create(Guid bingoId, Guid gameId, string code, string values, int bitmask)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("El código del cartón es obligatorio.", nameof(code));

        if (string.IsNullOrWhiteSpace(values))
            throw new ArgumentException("Los valores del cartón son obligatorios.", nameof(values));

        return new Card
        {
            Id = Guid.NewGuid(),
            BingoId = bingoId,
            GameId = gameId,
            Code = code.Trim().ToUpper(),
            PlayerId = null,
            Values = values,
            Bitmask = bitmask,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AssignPlayer(Guid playerId)
    {
        if (playerId == Guid.Empty)
            throw new ArgumentException("El Id del jugador es obligatorio.", nameof(playerId));

        if (PlayerId != null)
            throw new InvalidOperationException("Este cartón ya está asignado a un jugador.");

        PlayerId = playerId;
    }

    public void UnassignPlayer() => PlayerId = null;


    public void UpdateQrPath(string qrCodePath)
    {
        if (string.IsNullOrWhiteSpace(qrCodePath))
            throw new ArgumentException("La ruta del QR no puede estar vacía.", nameof(qrCodePath));
        QrCodePath = qrCodePath;
    }

    public void MarkNumber(int position)
    {
        if (position is < 0 or > 24)
            throw new ArgumentOutOfRangeException(nameof(position), "La posición debe estar entre 0 y 24.");
        Bitmask |= 1 << position;
    }
}