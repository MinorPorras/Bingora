using System.Text.Json;
using Bingora.Domain.ValueObjects;

namespace Bingora.Application.Helpers;

public static class BingoCardValueGenerator
{
    private static readonly Random _random = Random.Shared;

    /// <summary>
    /// Genera un JSON con 25 números (5 por cada rango de columna definido en la plantilla).
    /// </summary>
    /// <param name="ranges">Colección de Value Objects ColumnRanges que contiene los rangos personalizados de cada fila</param>
    public static string GenerateValues(IReadOnlyList<ColumnRange> ranges)
    {
        if (ranges.Count != 5)
            throw new ArgumentException(
                "Se esperan exactamente 5 rangos de columnas para generar un cartón estandar 5x5");
        var allNumbers = new List<int>(25);
        foreach (var range in ranges.OrderBy(r => r.MinValue))
        {
            var columnNumbers = new HashSet<int>();
            while (columnNumbers.Count < 5)
                columnNumbers.Add(_random.Next(range.MinValue, range.MaxValue + 1));
            allNumbers.AddRange(columnNumbers.OrderBy(n => n));
        }

        return JsonSerializer.Serialize(allNumbers);
    }
}