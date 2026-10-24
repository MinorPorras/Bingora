namespace Bingora.Domain.Entities;

public class Card
{
    public Guid Id { get; private set; }
    public Guid BingoId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string QrCodePath { get; private set; } = string.Empty;
    public string Values { get; private set; } = string.Empty; //Ej: "1,15,22..."
    public int Bitmask { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; set; }


    //Navegación
    public Bingo Bingo { get; private set; }
    public Player Player { get; private set; }

    private Card()
    {
    }

    public static Card Create(Guid bingoId, string code, string values, int bitmask)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("El código del cartón es obligatorio.", nameof(code));

        if (string.IsNullOrWhiteSpace(values))
            throw new ArgumentException("Los valores del cartón son obligatorios.", nameof(values));

        return new Card
        {
            Id = Guid.NewGuid(),
            BingoId = bingoId,
            Code = code.Trim().ToUpper(),
            Values = values,
            Bitmask = bitmask,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

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