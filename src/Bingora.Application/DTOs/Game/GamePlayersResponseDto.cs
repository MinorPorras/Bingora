using Bingora.Domain.Enums;

namespace Bingora.Application.DTOs.Game;

public record GamePlayersResponseDto(string GameTitle, GameStatus Status, IReadOnlyCollection<PlayerInfoDto> Players);