namespace Bingora.Application.DTOs.Game;

public record JoinWithCardRequestDto(string GameCode, string CardCode, string? AccessPassword, string PlayerName);