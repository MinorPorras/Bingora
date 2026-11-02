namespace Bingora.Application.Interfaces;

public interface IQrCodeService
{
    /// <summary>
    /// Genera un código QR para un cartón específico y lo guarda en disco.
    /// </summary>
    /// <param name="gameCode">Código único del juego</param>
    /// <param name="cardCode">Código único del cartón</param>
    /// <param name="gameId">ID del juego (para estructura de carpetas)</param>
    /// <returns>La ruta relativa del archivo QR generado</returns>
    Task<string> GenerateCardQrAsync(string gameCode, string cardCode, Guid gameId);

    /// <summary>
    /// Regenera todos los QRs de un juego.
    /// </summary>
    Task RegenerateGameQrAsync(Guid gameId, string gameCode, IEnumerable<string> cardCodes);
}