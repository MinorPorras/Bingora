namespace Bingora.Domain.Exceptions;

public class NoDistributedCardsException : DomainException
{
    public NoDistributedCardsException(Guid bingoId) : base(
        $"No se puede iniciar la partida. El bingo '{bingoId}' no tiene cartones distribuidos")
    { }
}