namespace Bingora.Application.DTOs.Game;

public record GamePatternResponseDto(
    Guid PatternId,
    string PatternName,
    int AvailablePrizes
);