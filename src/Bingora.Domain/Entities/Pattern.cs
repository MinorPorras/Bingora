using System.Text.RegularExpressions;

namespace Bingora.Domain.Entities;

public class Pattern
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string GridRepresentation { get; private set; } = string.Empty;
    public int Bitmask { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; } = true;

    private Pattern()
    {
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public static Pattern Create(string name, string gridRepresentation)
    {
        var pattern = new Pattern
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        pattern.Update(name, gridRepresentation);
        return pattern;
    }

    public void Update(string name, string gridRepresentation)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del patrón es obligatorio.", nameof(name));

        if (string.IsNullOrWhiteSpace(gridRepresentation))
            throw new ArgumentException("La representación de la cuadrícula es obligatoria.",
                nameof(gridRepresentation));

        if (!Regex.IsMatch(gridRepresentation, @"^[01]{25}$"))
            throw new ArgumentException(
                "El formato debe ser una cadena exacta de 25 caracteres compuesta solo por '0' y '1'.",
                nameof(gridRepresentation));

        Name = name.Trim();
        GridRepresentation = gridRepresentation;
        Bitmask = CalculateBitMask(gridRepresentation);
    }

    private static int CalculateBitMask(string grid)
    {
        var bitmask = 0;
        for (var i = 0; i < grid.Length; i++)
        {
            if (grid[i] == '1')
                bitmask |= (1 << i);
        }

        return bitmask;
    }
}