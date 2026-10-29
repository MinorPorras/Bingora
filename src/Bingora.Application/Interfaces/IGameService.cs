using Bingora.Application.DTOs.Game;
using Bingora.Domain.Common;

namespace Bingora.Application.Interfaces;

public interface IGameService
{
    /// <summary>
    /// Crea un nuevo Juego en la base de datos de forma asíncrona
    /// </summary>
    /// <param name="ownerId">ID del creador del juego</param>
    /// <param name="dto">El DTO enviado por el request ya validado previamente</param>
    /// <param name="ct">El token de cancelación</param>
    /// <returns></returns>
    Task<Result<GameResponseDto>> CreateGameAsync(Guid ownerId, CreateOrUpdateGameRequestDto dto,
        CancellationToken ct = default);

    /// <summary>
    /// Actualiza un juego ya existente y devuelve el DTO con toda la información actualizada
    /// </summary>
    /// <param name="gameId">El ID del juego a modificar</param>
    /// <param name="ownerId">El ID del usuario para el aislamiento de datos</param>
    /// <param name="dto">El DTO envíamos desde el request</param>
    /// <param name="ct">El token de cancelación</param>
    /// <returns></returns>
    Task<Result<GameResponseDto>> UpdateGameAsync(Guid gameId, Guid ownerId, CreateOrUpdateGameRequestDto dto,
        CancellationToken ct = default);

    /// <summary>
    /// Gestiona la generación de cartones para los juegos de bingo devolviendo la cantidad que se crearon
    /// y los códigos de cada uno para poder ser compartidos a los jugadores
    /// </summary>
    /// <param name="gameId">El ID del juego en el que se crearan los cartones</param>
    /// <param name="ownerId">El ID del dueño del usuario actual</param>
    /// <param name="quantity">La cantidad de cartones a generar</param>
    /// <param name="ct">El token de cancelación</param>
    /// <returns></returns>
    Task<Result<GeneratedCardsResponseDto>> GenerateCardAsync(Guid gameId, Guid ownerId, int quantity,
        CancellationToken ct = default);

    //Implementar StartGameAsync, DrawnNumberAsync, etc
}