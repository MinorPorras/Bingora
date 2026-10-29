using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Game;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Enums;
using FluentValidation;

namespace Bingora.Api.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/games").WithTags("Games")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.User), nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/{id:guid}",
                async (Guid id, IGameRepository repository, HttpContext context, CancellationToken ct) =>
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
                async (CreateOrUpdateGameRequestDto dto, IValidator<CreateOrUpdateGameRequestDto> validator,
                    IGameService service,
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
                async (Guid id, GenerateCardsRequestDto dto, IGameService service, HttpContext context,
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
                async (Guid id, CreateOrUpdateGameRequestDto dto, IValidator<CreateOrUpdateGameRequestDto> validator,
                    IGameService service, HttpContext context, CancellationToken ct) =>
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
            .WithDescription("Actualiza los datos de una partida específica.")
            .Produces<GameResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);
    }
}