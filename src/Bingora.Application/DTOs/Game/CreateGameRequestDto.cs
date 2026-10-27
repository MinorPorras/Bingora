namespace Bingora.Application.DTOs.Game;

public record CreateGameRequestDto(
    Guid BingoId,
    string Title,
    int AnimationDuration = 3,
    string? WinnerSoundPath = null
);