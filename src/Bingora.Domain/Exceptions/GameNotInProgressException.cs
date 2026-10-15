namespace Bingora.Domain.Exceptions;

public class GameNotInProgressException : DomainException
{
    public GameNotInProgressException(Guid gameId, string currentState) : base(
        $"No se puede generar un valor. La partida '{gameId}' está en estado '{currentState}'. Se quiere 'InProgress'.")
    { }
}