using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public static class GameErrors
{
    public static readonly Error NotFound = new("Game.NotFound",
        "El juego solicitado no existe.");

    public static Error ExceededMaxCards(int quantityToGenerate, int max) => new("Game.ExceededMaxCards",
        $"No se pueden generar {quantityToGenerate} cartones. Excederías el máximo de {max} cartones permitidos para esta plantilla."
    );
}