namespace Bingora.Application.DTOs.Game;

public record GeneratedCardsResponseDto(int GeneratedCount, IReadOnlyCollection<string> CardCodes);