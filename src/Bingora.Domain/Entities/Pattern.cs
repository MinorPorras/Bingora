using System.Text.RegularExpressions;

namespace Bingora.Domain.Entities;

public class Pattern
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    // Representación plana de 25 bits. Ej: "1000101010001000101010001"
    public string GridRepresentation { get; private set; } = string.Empty;
    public int Bitmask { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; }

    private Pattern()
    {
    }

    public Pattern Create(string name, string gridRepresentation)
    {
        var pattern = new Pattern
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };
        pattern.Update(name, gridRepresentation);
        return pattern;
    }

    public void Update(string name, string gridRepresentation)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "El nombre del patrón es obligatorio");
        if (string.IsNullOrWhiteSpace(gridRepresentation))
            throw new ArgumentNullException(nameof(gridRepresentation),
                "La representación de la cuadrícula es obligatoria");
        if (!Regex.IsMatch(gridRepresentation, @"^[01]{25}"))
            throw new ArgumentException("El formato de la cuadricula debe de ser 5 filas y 5 columnas",
                nameof(gridRepresentation));
        Name = name.Trim();
        GridRepresentation = gridRepresentation;
        Bitmask = CalculateBitMask(gridRepresentation);
    }

    private static int CalculateBitMask(string grid)
    {
        var bitmask = 0;
        for (var i = 0; i < grid.Length; i++)
            // Si el bot es 1, desplazamos el bit 1 a la posición 'i' y lo unimos con OR
            if (grid[i] == '1')
                bitmask |= (1 << i);
        return bitmask;
    }
}