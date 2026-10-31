namespace Bingora.Application.DTOs.Bingo;

public record BingoDetailDto(
    Guid Id,
    string Name,
    string Description,
    int TotalCards,
    DateTime CreatedAt,
    bool IsActive,
    IReadOnlyCollection<BingoPatternDto> Patterns
);