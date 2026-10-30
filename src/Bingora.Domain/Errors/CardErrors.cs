using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public static class CardErrors
{
    public static readonly Error NotFound = new("Card.NotFound", "El cartón especificado no existe en esta partida.");

    public static readonly Error AlreadyAssigned =
        new("Card.AlreadyAssigned", "Este cartón ya está asignado a otro jugador.");
}