namespace Bingora.Domain.Exceptions;
// Cuando buscas una entidad que DEBERÍA existir pero no está
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id) : base(
        $"La entidad '{entityName}' con ID '{id}' no fue encontrada.'"){}
}