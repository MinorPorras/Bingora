using Bingora.Application.DTOs.Game;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Errors;

namespace Bingora.Application.Services;

public class GameService(IBingoRepository bingoRepository, IGameRepository gameRepository) : IGameService
{
    public async Task<Result<GameResponseDto>> CreateGameAsync(Guid ownerId, CreateGameRequestDto dto,
        CancellationToken ct = default)
    {
        var bingo = await bingoRepository.GetByIdAsync(dto.BingoId, ownerId, ct);
        if (bingo is null) return Result<GameResponseDto>.Failure(BingoErrors.NotFound);
        var game = Game.Create(dto.BingoId, dto.Title, dto.AnimationDuration, dto.WinnerSoundPath);
        foreach (var pattern in bingo.BingoPatterns)
            game.AddPattern(pattern.PatternId, pattern.AvailablePrizes);

        await gameRepository.AddAsync(game, ct);
        var savedGame = await gameRepository.GetByIdWithDetailsAsync(game.Id, ownerId, ct);
        return Result<GameResponseDto>.Success(savedGame!.ToResponseDto());
    }
}