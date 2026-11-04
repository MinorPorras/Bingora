using Bingora.Application.DTOs.Game;
using Bingora.Application.Helpers;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Enums;
using Bingora.Domain.Errors;

namespace Bingora.Application.Services;

public class GameService(IBingoRepository bingoRepository, IGameRepository gameRepository, IQrCodeService qrCodeService)
    : IGameService
{
    public async Task<Result<GameResponseDto>> CreateGameAsync(Guid ownerId, CreateOrUpdateGameRequestDto dto,
        CancellationToken ct = default)
    {
        var bingo = await bingoRepository.GetByIdAsync(dto.BingoId, ownerId, ct);
        if (bingo is null) return Result<GameResponseDto>.Failure(BingoErrors.NotFound);
        var game = Game.Create(dto.BingoId, dto.Title, dto.AnimationDuration, dto.WinnerSoundPath, dto.AccessPassWord);
        foreach (var pattern in bingo.BingoPatterns)
            game.AddPattern(pattern.PatternId, pattern.AvailablePrizes);

        await gameRepository.AddAsync(game, ct);
        var savedGame = await gameRepository.GetByIdWithDetailsAsync(game.Id, ownerId, ct);
        return Result<GameResponseDto>.Success(savedGame!.ToResponseDto());
    }


    public async Task<Result<GameResponseDto>> UpdateGameAsync(Guid gameId, Guid ownerId,
        CreateOrUpdateGameRequestDto dto,
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
        var newCards = game.GenerateCards(quantity, generateValuesFunc).ToList();

        var cardsWithQr = new List<CardInfoDto>();
        foreach (var card in newCards)
        {
            var qrPath = await qrCodeService.GenerateCardQrAsync(game.GameCode, card.Code, gameId);
            card.UpdateQrPath(qrPath);
            cardsWithQr.Add(new CardInfoDto(card.Code, qrPath));
        }

        await gameRepository.AddCardsAsync(gameId, newCards, ct);

        return Result<GeneratedCardsResponseDto>.Success(new GeneratedCardsResponseDto(quantity, cardsWithQr));
    }

    public async Task<Result<int>> RegenerateQrAsync(Guid gameId, Guid ownerId, CancellationToken ct = default)
    {
        var game = await gameRepository.GetByIdAsync(gameId, ownerId, ct);
        if (game is null)
            return Result<int>.Failure(GameErrors.NotFound);
        var cardCodes = game.Cards.Select(c => c.Code).ToList();
        if (cardCodes.Count == 0)
            return Result<int>.Failure(GameErrors.NoCards);
        await qrCodeService.RegenerateGameQrAsync(game.Id, game.GameCode, cardCodes);
        return Result<int>.Success(cardCodes.Count);
    }

    public async Task<Result<JoinGameResponseDto>> JoinGameAsync(JoinGameRequestDto dto, CancellationToken ct = default)
    {
        var game = await gameRepository.GetByCodeAsync(dto.GameCode, ct);
        if (game is null) return Result<JoinGameResponseDto>.Failure(GameErrors.NotFound);
        if (!game.VerifyJoinPassword(dto.AccessPassword))
            return Result<JoinGameResponseDto>.Failure(GameErrors.InvalidPassword);
        if (game.Status == GameStatus.Finished)
            return Result<JoinGameResponseDto>.Failure(GameErrors.GameFinished);
        var availableCard = game.Cards.FirstOrDefault(c => c.Player == null);
        if (availableCard is null)
            return Result<JoinGameResponseDto>.Failure(GameErrors.NoCardsAvailable);
        var newPlayer = game.AddPlayer(dto.PlayerName, availableCard);
        await gameRepository.AddPlayerAsync(newPlayer, ct);

        return Result<JoinGameResponseDto>.Success(new JoinGameResponseDto(
            newPlayer.Id,
            dto.PlayerName,
            availableCard.Code,
            game.Title)
        );
    }

    public async Task<Result<JoinGameResponseDto>> JoinGameWithCardAsync(JoinWithCardRequestDto dto,
        CancellationToken ct)
    {
        var game = await gameRepository.GetByCodeAsync(dto.GameCode, ct);
        if (game is null) return Result<JoinGameResponseDto>.Failure(GameErrors.InvalidCode);

        if (!game.VerifyJoinPassword(dto.AccessPassword))
            return Result<JoinGameResponseDto>.Failure(GameErrors.InvalidPassword);

        if (game.Status == GameStatus.Finished)
            return Result<JoinGameResponseDto>.Failure(GameErrors.GameFinished);

        var card = game.Cards.FirstOrDefault(c =>
            c.Code.Equals(dto.CardCode, StringComparison.InvariantCultureIgnoreCase));
        if (card is null)
            return Result<JoinGameResponseDto>.Failure(CardErrors.NotFound);

        if (card.Player is not null)
            return Result<JoinGameResponseDto>.Failure(CardErrors.AlreadyAssigned);

        var newPlayer = game.AddPlayer(dto.PlayerName, card);
        await gameRepository.AddPlayerAsync(newPlayer, ct);
        return Result<JoinGameResponseDto>.Success(new JoinGameResponseDto(
            newPlayer.Id,
            dto.PlayerName,
            card.Code,
            game.Title)
        );
    }

    public async Task<Result> LeaveGameAsync(LeaveGameRequestDto dto, CancellationToken ct = default)
    {
        var game = await gameRepository.GetByIdForManagementAsync(dto.GameId, ct);
        if (game is null) return Result.Failure(GameErrors.NotFound);

        if (game.Players.All(p => p.Id != dto.PlayerId))
            return Result.Failure(PlayerErrors.NotFound);

        if (game.Status == GameStatus.Finished)
            return Result.Failure(GameErrors.GameFinished);

        game.RemovePlayer(dto.PlayerId);
        await gameRepository.UpdateAsync(game, ct);

        return Result.Success();
    }

    public async Task<Result> KickPlayerAsync(Guid gameId, Guid playerId, Guid userId,
        CancellationToken ct = default)
    {
        var game = await gameRepository.GetByIdForManagementAsync(gameId, ct);
        if (game is null || game.Bingo.OwnerId != userId)
            return Result.Failure(GameErrors.NotFound);

        if (game.Players.All(p => p.Id != playerId))
            return Result.Failure(PlayerErrors.NotFound);

        game.RemovePlayer(playerId);
        await gameRepository.UpdateAsync(game, ct);

        return Result.Success();
    }

    public async Task<Result<GamePlayersResponseDto>> GetPlayersAsync(Guid gameId, Guid ownerId, CancellationToken ct)
    {
        var game = await gameRepository.GetByIdForManagementAsync(gameId, ct);
        if (game is null || game.Bingo.OwnerId != ownerId)
            return Result<GamePlayersResponseDto>.Failure(GameErrors.NotFound);

        var playersDto = game.Players.Select(p => new PlayerInfoDto(
            p.Id,
            p.Name,
            p.Card?.Code ?? "Sin cartón"
        )).ToList();

        return Result<GamePlayersResponseDto>.Success(new GamePlayersResponseDto(
            game.Title,
            game.Status,
            playersDto
        ));
    }
}