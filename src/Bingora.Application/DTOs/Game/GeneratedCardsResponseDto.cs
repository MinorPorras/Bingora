namespace Bingora.Application.DTOs.Game;

public record GeneratedCardsResponseDto(int GeneratedCount, IReadOnlyCollection<CardInfoDto> CardCodes);