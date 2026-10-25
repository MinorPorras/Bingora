namespace Bingora.Application.DTOs.Bingo;

public record PatternManagementResponseDto(Guid PatternId, string Name, int AvailablePrizes);