using Bingora.Application.DTOs.Bingo;
using Bingora.Domain.Entities;
using Bingora.Domain.ValueObjects;

namespace Bingora.Application.Mappings;

public static class BingoMappings
{
    public static ColumnRangeDto ToDto(this ColumnRange columnRange)
    {
        return new ColumnRangeDto(
            columnRange.ColumnName,
            columnRange.MinValue,
            columnRange.MaxValue
        );
    }

    public static BingoResponseDto ToResponseDto(this Bingo bingo)
    {
        return new BingoResponseDto(
            bingo.Id,
            bingo.Name,
            bingo.Description,
            bingo.TotalCards,
            bingo.CreatedAt,
            bingo.OwnerId,
            bingo.ColumnRanges.Select(cr => cr.ToDto()).ToList(),
            bingo.IsActive
        );
    }

    public static PatternManagementResponseDto ToResponseDto(this BingoPattern bingoPattern)
    {
        return new PatternManagementResponseDto(
            bingoPattern.PatternId,
            bingoPattern.Pattern.Name,
            bingoPattern.AvailablePrizes
        );
    }
}