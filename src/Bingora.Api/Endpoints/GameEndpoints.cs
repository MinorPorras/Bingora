using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Game;
using Bingora.Application.Interfaces;
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

        group.MapPost("/",
                async (CreateGameRequestDto dto, IValidator<CreateGameRequestDto> validator, IGameService service,
                    HttpContext context, CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid) return ProblemResults.Validation(validationResult.ToDictionary());

                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();

                    var response = await service.CreateGameAsync(userId, dto, ct);

                    return response.IsFailure
                        ? ProblemResults.NotFound("Error creando el juego", response.Error?.Message)
                        : Results.Created($"/api/games/{response.Value!.Id}", response);
                })
            .WithSummary("Crear nueva partida")
            .WithDescription("Crea una nueva partida de bingo basada en una plantilla de bingo seleccionada")
            .Produces<GameResponseDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);
    }
}