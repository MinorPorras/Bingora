namespace Bingora.Application.DTOs.Game;

public record JoinGameResponseDto(Guid PlayerId, string PlayerName, string CardCode, string GameTitle);