using Bingora.Domain.ValueObjects;

namespace Bingora.Application.DTOs.Bingo;

public record BingoSummaryDto(
    Guid Id,
    string Name,
    string Description,
    int TotalCards,
    DateTime CreatedAt,
    bool IsActive);