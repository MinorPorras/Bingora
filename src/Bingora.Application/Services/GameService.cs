using Bingora.Application.DTOs.Game;
using Bingora.Application.Helpers;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Errors;

namespace Bingora.Application.Services;

public class GameService(IBingoRepository bingoRepository, IGameRepository gameRepository) : IGameService
{

    public async Task<Result<GameResponseDto>> CreateGameAsync(Guid ownerId, CreateOrUpdateGameRequestDto dto,
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


    public async Task<Result<GameResponseDto>> UpdateGameAsync(Guid gameId, Guid ownerId, CreateOrUpdateGameRequestDto dto,
        CancellationToken ct = default)
    {
        var game = await gameRepository.GetByIdAsync(gameId, ownerId, ct);
        if (game is null) return Result<GameResponseDto>.Failure(GameErrors.NotFound);
        game.UpdateConfiguration(dto.Title, dto.AnimationDuration, dto.WinnerSoundPath, dto.AccessPassWord);
        await gameRepository.UpdateAsync(game, ct);
        var savedGame = await gameRepository.GetByIdWithDetailsAsync(game.Id, ownerId, ct);
        return Result<GameResponseDto>.Success(savedGame!.ToResponseDto());
    }

    public async Task<Result<GeneratedCardsResponseDto>> GenerateCardAsync(Guid gameId, Guid ownerId, int quantity,
        CancellationToken ct = default)
    {
        var game = await gameRepository.GetByIdAsync(gameId, ownerId, ct);
        if (game is null) return Result<GeneratedCardsResponseDto>.Failure(GameErrors.NotFound);

        var bingo = await bingoRepository.GetByIdAsync(game.BingoId, ownerId, ct);
        if (bingo is null) return Result<GeneratedCardsResponseDto>.Failure(BingoErrors.NotFound);

        if (game.Cards.Count + quantity > bingo.TotalCards)
            return Result<GeneratedCardsResponseDto>.Failure(GameErrors.ExceededMaxCards(quantity, bingo.TotalCards));

        Func<string> generateValuesFunc = () => BingoCardValueGenerator.GenerateValues(bingo.ColumnRanges);

        game.GenerateCards(quantity, generateValuesFunc);

        await gameRepository.UpdateAsync(game, ct);
        var newCodes = game.Cards
            .OrderByDescending(c => c.CreatedAt)
            .Take(quantity)
            .Select(c => c.Code)
            .ToList();
        return Result<GeneratedCardsResponseDto>.Success(new GeneratedCardsResponseDto(quantity, newCodes));
    }
}