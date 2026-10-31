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

    public static BingoSummaryDto ToSummaryDto(this Bingo bingo) => new(
        bingo.Id,
        bingo.Name,
        bingo.Description,
        bingo.TotalCards,
        bingo.CreatedAt,
        bingo.IsActive
    );

    public static BingoDetailDto ToDetailDto(this Bingo bingo) => new(
        bingo.Id,
        bingo.Name,
        bingo.Description,
        bingo.TotalCards,
        bingo.CreatedAt,
        bingo.IsActive,
        [
            .. bingo.BingoPatterns.Select(bp => new BingoPatternDto(
                bp.PatternId,
                bp.Pattern.Name,
                bp.AvailablePrizes
            ))
        ]
    );

    public static PatternManagementResponseDto ToResponseDto(this BingoPattern bingoPattern)
    {
        return new PatternManagementResponseDto(
            bingoPattern.PatternId,
            bingoPattern.Pattern.Name,
            bingoPattern.AvailablePrizes
        );
    }
}