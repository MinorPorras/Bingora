using Bingora.Domain.Enums;

namespace Bingora.Domain.Entities;

public class Winner
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid PatternId { get; private set; }
    public PrizeClaimStatus ClaimStatus { get; private set; } = PrizeClaimStatus.Unclaimed;
    public DateTime WonAt { get; private set; }
    public bool IsActive { get; private set; }

    //Navegación
    public Game Game { get; private set; }
    public Player Player { get; private set; }
    public Pattern Pattern { get; private set; }

    private Winner()
    {
    }

    public static Winner Create(Guid gameId, Guid playerId, Guid patternId) => new Winner
    {
        Id = Guid.NewGuid(),
        GameId = gameId,
        PlayerId = playerId,
        PatternId = patternId,
        ClaimStatus = PrizeClaimStatus.Unclaimed,
        WonAt = DateTime.UtcNow
    };

    public void MarkAsClaimed() =>
        ClaimStatus = ClaimStatus switch
        {
            PrizeClaimStatus.Claimed => throw new InvalidOperationException(
                "Este premio ya fue reclamado anteriormente."),
            PrizeClaimStatus.Rejected => throw new InvalidOperationException(
                "No se puede reclamar un premio que ha sido rechazado."),
            _ => PrizeClaimStatus.Claimed
        };

    public void MarkAsPendingVerification()
    {
        if (ClaimStatus != PrizeClaimStatus.Unclaimed)
            throw new InvalidOperationException("Solo se puede poner en verificación un premio no reclamado.");
        ClaimStatus = PrizeClaimStatus.PendingVerification;
    }

    //A futuro se puede agregar una propiedad RejectionReason si es necesaria para indicar al razón del rechazo de la entrega del premio
    public void Reject() => ClaimStatus = PrizeClaimStatus.Rejected;
}