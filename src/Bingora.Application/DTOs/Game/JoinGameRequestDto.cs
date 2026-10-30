namespace Bingora.Application.DTOs.Game;

public record JoinGameRequestDto(string GameCode, string? AccessPassword, string PlayerName);