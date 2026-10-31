namespace Bingora.Application.DTOs.Bingo;

public record BingoPatternDto(
    Guid PatternId,
    string PatternName,
    int AvailablePrizes
);