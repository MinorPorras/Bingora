using Bingora.Domain.ValueObjects;

namespace Bingora.Application.DTOs.Bingo;

public record BingoResponseDto(
    Guid Id,
    string Name,
    string Description,
    int TotalCards,
    DateTime CreatedAt,
    Guid OwnerId,
    IReadOnlyCollection<ColumnRangeDto> ColumnRanges,
    bool IsActive);