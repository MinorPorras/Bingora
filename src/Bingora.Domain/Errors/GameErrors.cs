using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public static class GameErrors
{
    public static readonly Error NotFound = new("Game.NotFound",
        "El juego solicitado no existe.");

    public static Error ExceededMaxCards(int quantityToGenerate, int max) => new("Game.ExceededMaxCards",
        $"No se pueden generar {quantityToGenerate} cartones. Excederías el máximo de {max} cartones permitidos para esta plantilla."
    );

    public static readonly Error InvalidCode = new("Game.InvalidCode", "El código de la partida no es válido.");

    public static readonly Error
        InvalidPassword = new("Game.InvalidPassword", "La contraseña de acceso es incorrecta.");

    public static readonly Error GameFinished =
        new("Game.Finished", "No se puede unir a una partida que ya ha finalizado.");

    public static readonly Error NoCardsAvailable =
        new("Game.NoCards", "No hay cartones disponibles para asignar. El juego está lleno.");
}