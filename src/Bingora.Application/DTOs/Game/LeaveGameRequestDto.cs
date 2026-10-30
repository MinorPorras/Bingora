namespace Bingora.Application.DTOs.Game;

public record LeaveGameRequestDto(Guid PlayerId, Guid GameId);