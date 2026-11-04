namespace Bingora.Application.DTOs.Game;

public record RegenerateQRsResponseDto(
    string Message,
    int RegeneratedCount
);