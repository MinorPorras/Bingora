using Bingora.Application.DTOs.Game;
using Bingora.Domain.Entities;

namespace Bingora.Application.Mappings;

public static class GameMappings
{
    public static GamePatternResponseDto ToResponseDto(this GamePattern gp) =>
        new GamePatternResponseDto(gp.PatternId, gp.Pattern.Name, gp.TotalPrizes);

    public static GameResponseDto ToResponseDto(this Game game) => new GameResponseDto(
        game.Id,
        game.BingoId,
        game.Title,
        game.Status.ToString(),
        game.CreatedAt,
        game.StartedAt,
        game.FinishedAt,
        game.AnimationDuration,
        game.WinnerSoundPath,
        game.GamePatterns.Select(gp => gp.ToResponseDto()).ToList()
    );
}