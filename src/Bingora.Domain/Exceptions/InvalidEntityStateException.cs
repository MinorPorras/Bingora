namespace Bingora.Domain.Exceptions;
// Cuando una entidad está en un estado que no permite la operación
public class InvalidEntityStateException : DomainException
{
    public InvalidEntityStateException(string entityName, string currentState, string requiredState) : base(
        $"La entidad '{entityName}' está en estado '{currentState}' pero se requiere '{requiredState}'") { }
}