namespace Bingora.Domain.Exceptions;
// Cuando se viola una regla de negocio crítica
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string rule): base($"Violación de regla de negocio: {rule}") { }
}