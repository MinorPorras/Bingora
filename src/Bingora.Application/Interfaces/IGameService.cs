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
    /// <returns>Patrón Result con un GameResponseDto</returns>
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
    /// <returns>Patrón Result con un GeneratedCardsResponseDto</returns>
    Task<Result<GeneratedCardsResponseDto>> GenerateCardAsync(Guid gameId, Guid ownerId, int quantity,
        CancellationToken ct = default);

    /// <summary>
    /// Permite que un nuevo jugador se pueda contectar a una partida por medio del código de esta
    /// </summary>
    /// <param name="dto">Data Transfer Object con los datos necesarios</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Patrón Result con un JoinGameResponseDto</returns>
    Task<Result<JoinGameResponseDto>> JoinGameAsync(JoinGameRequestDto dto, CancellationToken ct = default);

    /// <summary>
    /// Permite que los usuarios con un cartón ya asignado puedan unirse a una partida directamente usando el cartón que le pertenece
    /// </summary>
    /// <param name="dto">Data Transfer Object con los datos necesarios</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Patrón Result con un JoinGameResponseDto</returns>
    Task<Result<JoinGameResponseDto>> JoinGameWithCardAsync(JoinWithCardRequestDto dto, CancellationToken ct);

    /// <summary>
    /// Permite a los jugadores que ya forman parte de una partida el poder salir de esta en cualquier momento
    /// </summary>
    /// <param name="dto">DTO con el PlayerId del jugador</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Patrón Result simple</returns>
    Task<Result> LeaveGameAsync(LeaveGameRequestDto dto, CancellationToken ct = default);

    /// <summary>
    /// Permite que el creador de la partida pueda expulsar a otros jugadores de la partida
    /// </summary>
    /// <param name="dto">DTO con el PlayerId del jugador a expulsar</param>
    /// <param name="ct"></param>
    /// <returns>Patrón Result simple</returns>
    Task<Result> KickPlayerAsync(Guid gameId, Guid playerId, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Obtiene la información general de la partida previa a su inicio, incluido un listado de los jugadores
    /// que actualmente están registrados en el juego
    /// </summary>
    /// <param name="gameId">ID de de la partida</param>
    /// <param name="ownerId">Id del usuario creador de la partida</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Patrón result con la información de los jugadores</returns>
    Task<Result<GamePlayersResponseDto>> GetPlayersAsync(Guid gameId, Guid ownerId, CancellationToken ct);


    //Implementar StartGameAsync, DrawnNumberAsync, etc
}