namespace Bingora.Application.DTOs.Game;

public record CreateOrUpdateGameRequestDto(
    Guid BingoId,
    string Title,
    int AnimationDuration = 3,
    string? WinnerSoundPath = null,
    string? AccessPassWord = null
);