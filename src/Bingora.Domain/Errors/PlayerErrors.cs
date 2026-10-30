using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public class PlayerErrors
{
    public static readonly Error NotFound = new("Player.NotFound", "El jugador no se encuentra en la partida.");

    public static readonly Error AlreadyJoined =
        new("Player.AlreadyJoined", "Ya te has unido a esta partida con este nombre.");
}