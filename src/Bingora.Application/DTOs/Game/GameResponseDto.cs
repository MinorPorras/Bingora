namespace Bingora.Application.DTOs.Game;

public record GameResponseDto(
    Guid Id,
    Guid BingoId,
    string Title,
    string Status,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    int AnimationDuration,
    string? WinnerSoundPath,
    IReadOnlyCollection<GamePatternResponseDto> Patterns
);