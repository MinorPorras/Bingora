using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Game;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Bingora.Api.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/games").WithTags("Games")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.User), nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        //---------------------------- Configuración ------------------------------------//

        group.MapGet("/{id:guid}",
                async (Guid id, [FromServices] IGameRepository repository, HttpContext context, CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var game = await repository.GetByIdWithDetailsAsync(id, userId, ct);
                    return game is null
                        ? ProblemResults.NotFound("El juego buscado no existe")
                        : Results.Ok(game.ToResponseDto());
                })
            .WithSummary("Obtener una partida")
            .WithDescription("Obtiene los datos de un bingo incluyendo los patrones que este contiene")
            .Produces<GameResponseDto>(200)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/",
                async ([FromBody] CreateOrUpdateGameRequestDto dto,
                    [FromServices] IValidator<CreateOrUpdateGameRequestDto> validator,
                    [FromServices] IGameService service,
                    HttpContext context, CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid) return ProblemResults.Validation(validationResult.ToDictionary());

                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();

                    var response = await service.CreateGameAsync(userId, dto, ct);

                    if (response.IsFailure)
                    {
                        return response.Error?.Code == "Bingo.NotFound"
                            ? ProblemResults.NotFound(response.Error.Message)
                            : ProblemResults.BadRequest(response.Error.Message);
                    }

                    return Results.Created($"/api/games/{response.Value!.Id}", response.Value);
                })
            .WithSummary("Crear nueva partida")
            .WithDescription("Crea una nueva partida de bingo basada en una plantilla de bingo seleccionada")
            .Produces<GameResponseDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/cards",
                async (Guid id, [FromBody] GenerateCardsRequestDto dto, [FromServices] IGameService service,
                    HttpContext context,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    if (dto.Quantity <= 0)
                        return ProblemResults.BadRequest("La cantidad de cartones debe de ser mayor a 0.");
                    var response = await service.GenerateCardAsync(id, userId, dto.Quantity, ct);
                    if (response.IsSuccess) return Results.Ok(response.Value);

                    var isNotFound = response.Error?.Code is "Game.NotFound" or "Bingo.NotFound";
                    return isNotFound
                        ? ProblemResults.NotFound(response.Error!.Message)
                        : ProblemResults.BadRequest(response.Error!.Message);
                })
            .WithSummary("Generar cartones para una partida")
            .WithDescription(
                "Genera una cantidad específica de cartones nuevos para la partida, respetando los rangos de la plantilla.")
            .Produces<GeneratedCardsResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPut("/{id:guid}",
                async (Guid id, [FromBody] CreateOrUpdateGameRequestDto dto,
                    [FromServices] IValidator<CreateOrUpdateGameRequestDto> validator,
                    [FromServices] IGameService service, HttpContext context, CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid) return ProblemResults.Validation(validationResult.ToDictionary());
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var response = await service.UpdateGameAsync(id, userId, dto, ct);
                    if (response.IsFailure)
                    {
                        return response.Error?.Code == "Game.NotFound"
                            ? ProblemResults.NotFound(response.Error.Message)
                            : ProblemResults.BadRequest(response.Error.Message);
                    }

                    return Results.Ok(response.Value);
                })
            .WithSummary("Actualiza una partida")
            .WithDescription("Actualiza la configuración de la partida de una partida específica.")
            .Produces<GameResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        //---------------------------- Join ------------------------------------//

        group.MapPost("/join",
                async ([FromBody] JoinGameRequestDto dto, [FromServices] IValidator<JoinGameRequestDto> validator,
                    [FromServices] IGameService service,
                    CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid) return ProblemResults.Validation(validationResult.ToDictionary());

                    var response = await service.JoinGameAsync(dto, ct);
                    if (response.IsFailure)
                        return response.Error?.Code is "Game.InvalidCode" or "Game.InvalidPassword"
                            ? ProblemResults.Unauthorized(response.Error.Message) // 401 para credenciales malas
                            : ProblemResults.BadRequest(response.Error.Message); // 400 para juego lleno/terminado

                    return Results.Ok(response.Value);
                })
            .WithSummary("Unirse a un Partida")
            .Produces<JoinGameResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .AllowAnonymous();

        group.MapPost("/players/Leave",
                async ([FromBody] LeaveGameRequestDto dto, [FromServices] IGameService service, CancellationToken ct) =>
                {
                    var response = await service.LeaveGameAsync(dto, ct);
                    return response.IsFailure
                        ? ProblemResults.BadRequest(response.Error.Message)
                        : Results.Ok(new { Message = "Has salido de la partida exitosamente." });
                })
            .WithSummary("Salirse de una partida")
            .Produces(200)
            .ProducesProblem(400)
            .AllowAnonymous();

        group.MapDelete("/{gameId:guid}/players/{playerId:guid}",
                async (Guid gameId, Guid playerId, [FromServices] IGameService service, HttpContext context,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var dto = new LeaveGameRequestDto(gameId, playerId);
                    var response = await service.KickPlayerAsync(dto, ct);
                    if (response.IsFailure)
                        return response.Error?.Code == "Game.NotFound"
                            ? ProblemResults.NotFound(response.Error.Message)
                            : ProblemResults.BadRequest(response.Error.Message);

                    return Results.NoContent();
                })
            .WithSummary("Echar a un jugador de la partida (Solo Admin)")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapGet("/{id:guid}/players",
                async (Guid id, [FromServices] IGameService service, HttpContext context, CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var response = await service.GetPlayersAsync(id, userId, ct);
                    if (response.IsFailure)
                    {
                        return response.Error?.Code == "Game.NotFound"
                            ? ProblemResults.NotFound(response.Error.Message)
                            : ProblemResults.BadRequest(response.Error.Message);
                    }

                    return Results.Ok(response.Value);
                })
            .WithSummary("Obtener lista de jugadores de la partida")
            .WithDescription(
                "Devuelve el nombre y código de cartón de todos los jugadores que se han unido a la partida.")
            .Produces<GamePlayersResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);
    }
}