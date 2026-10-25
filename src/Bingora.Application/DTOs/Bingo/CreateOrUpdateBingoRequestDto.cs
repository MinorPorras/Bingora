using Bingora.Domain.ValueObjects;

namespace Bingora.Application.DTOs.Bingo;

public record CreateOrUpdateBingoRequestDto(
    string Name,
    string Description,
    int TotalCards,
    IReadOnlyCollection<ColumnRangeDto> ColumnRanges);