namespace Bingora.Domain.Exceptions;

public class InsufficientValuesForCardException : DomainException
{
    public InsufficientValuesForCardException(int availableValues, int requieredValues) : base(
        $"No hay suficientes valores para generar el cartón. Disponibles: {availableValues}. Requeridos: {requieredValues}")
    { }
}