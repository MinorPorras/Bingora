namespace Bingora.Application.DTOs.Game;

public record CardSummaryDto(
    Guid Id,
    string Code,
    string QrCodeUrl,
    bool IsAssigned,
    DateTime CreatedAt
);