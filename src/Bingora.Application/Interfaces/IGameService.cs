using Bingora.Application.DTOs.Game;
using Bingora.Domain.Common;

namespace Bingora.Application.Interfaces;

public interface IGameService
{
    Task<Result<GameResponseDto>> CreateGameAsync(Guid ownerId, CreateGameRequestDto dto,
        CancellationToken ct = default);

    //Implementar StartGameAsync, DrawnNumberAsync, etc
}