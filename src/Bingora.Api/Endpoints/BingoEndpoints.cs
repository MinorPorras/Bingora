using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Bingo;
using Bingora.Application.DTOs.Common;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Enums;
using Bingora.Domain.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Bingora.Api.Endpoints;

public static class BingoEndpoints
{
    public static void MapBingoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bingos").WithTags("Bingo")
            .RequireAuthorization(p => p.RequireRole(nameof(Role.User), nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/",
                async ([AsParameters] PaginatedQuery query, HttpContext context,
                    [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                    var paged = await repository.GetPagedAsync(userId, query.PageNumber, query.PageSize, ct);
                    return Results.Ok(paged.MapTo(b => b.ToSummaryDto()));
                })
            .WithSummary("Obtener todos los bingos")
            .WithDescription("Obtiene una lista paginada de todos los bingos.")
            .Produces<PaginatedResult<BingoSummaryDto>>(200)
            .ProducesProblem(401);

        group.MapGet("/trash",
                async ([AsParameters] PaginatedQuery query, HttpContext context,
                    [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                    var paged = await repository.GetPagedAsyncIgnoringFilters(userId, query.PageNumber, query.PageSize,
                        ct);
                    return Results.Ok(paged.MapTo(b => b.ToSummaryDto()));
                })
            .WithSummary("Obtiene todos los bingo inactivos.")
            .WithDescription("Obtiene todos los bingos que el usuario haya desactivado previamente.")
            .Produces<PaginatedResult<BingoSummaryDto>>(200)
            .ProducesProblem(401);

        group.MapGet("/search/{name}",
                async (string name, [AsParameters] PaginatedQuery query, HttpContext context,
                    [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                    var paged = await repository.GetPagedByNameOrDescriptionAsync(userId, name, query.PageNumber,
                        query.PageSize,
                        ct);
                    return Results.Ok(paged.MapTo(b => b.ToSummaryDto()));
                })
            .WithSummary("Obtiene los bingos por nombre")
            .WithDescription("Obtiene todos los bingos que contengan en su nombre el valor buscado.")
            .Produces<PaginatedResult<BingoSummaryDto>>(200)
            .ProducesProblem(401);

        group.MapGet("/{id:guid}",
                async (Guid id, HttpContext context, [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                    var result = await repository.GetByIdAsync(id, userId, ct);
                    return result is null
                        ? ProblemResults.NotFound()
                        : Results.Ok(result.ToDetailDto());
                })
            .WithSummary("Obtiene un bingo por su ID")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapGet("/trash-details/{id:guid}/",
                async (Guid id, HttpContext context, [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                    var result = await repository.GetByIdAsyncIgnoringFilters(id, userId, ct);
                    return result is null
                        ? ProblemResults.NotFound()
                        : Results.Ok(result.ToDetailDto());
                })
            .WithSummary("Obtiene un bingo inactivo por su ID")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/",
                async ([FromBody] CreateOrUpdateBingoRequestDto dto,
                    [FromServices] IValidator<CreateOrUpdateBingoRequestDto> validator,
                    HttpContext context, [FromServices] IBingoRepository repository, CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid)
                        return ProblemResults.Validation(validationResult.ToDictionary());
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var columnRanges = dto.ColumnRanges
                        .Select(cr => new ColumnRange(cr.ColumnName, cr.MinValue, cr.MaxValue))
                        .ToList();
                    var bingo = Bingo.Create(
                        dto.Name,
                        dto.Description,
                        dto.TotalCards,
                        userId,
                        columnRanges
                    );
                    await repository.AddAsync(bingo, ct);
                    return Results.Created($"/api/bingos/{bingo.Id}", bingo.ToDetailDto());
                })
            .WithSummary("Crear un Bingo")
            .WithDescription("Permite crear un nuevo bingo con sus datos básicos de plantilla para crear partidas.")
            .Produces<BingoSummaryDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(401);

        group.MapPut("/{id:guid}",
                async (Guid id, [FromBody] CreateOrUpdateBingoRequestDto dto,
                    [FromServices] IValidator<CreateOrUpdateBingoRequestDto> validator,
                    HttpContext context,
                    [FromServices] IBingoRepository repository, CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid)
                        return ProblemResults.Validation(validationResult.ToDictionary());
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var bingo = await repository.GetByIdAsync(id, userId, ct);
                    if (bingo == null)
                        return ProblemResults.NotFound();
                    var columnRanges = dto.ColumnRanges
                        .Select(cr => new ColumnRange(cr.ColumnName, cr.MinValue, cr.MaxValue))
                        .ToList();
                    bingo.UpdateConfiguration(dto.Name, dto.Description, dto.TotalCards, columnRanges);
                    await repository.UpdateAsync(bingo, ct);
                    return Results.Ok(bingo.ToDetailDto());
                })
            .WithSummary("Actualiza un bingo")
            .WithDescription("Permite actualizar la configuración d un bingo.")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/restore",
                async (Guid id, HttpContext context, [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var bingo = await repository.GetByIdAsyncIgnoringFilters(id, userId, ct);
                    if (bingo == null)
                        return ProblemResults.NotFound();
                    bingo.Restore();
                    await repository.UpdateAsync(bingo, ct);
                    return Results.Ok();
                })
            .WithSummary("Activa un bingo inactivo")
            .WithDescription("Permite activar un bingo previamente desactivado por el usuario")
            .Produces(201)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/soft-delete",
                async (Guid id, HttpContext context, [FromServices] IBingoRepository repository,
                    CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId))
                        return ProblemResults.Unauthorized();
                    var bingo = await repository.GetByIdAsync(id, userId, ct);
                    if (bingo == null)
                        return ProblemResults.NotFound();
                    bingo.SoftDelete();
                    await repository.UpdateAsync(bingo, ct);
                    return Results.Ok();
                })
            .WithSummary("Desactiva un bingo")
            .WithDescription("Permite desactivar un bingo especificado por el usuario")
            .Produces(201)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/{id:guid}/patterns", async (Guid id, [FromBody] PatternManagementRequestDto dto,
                [FromServices] IValidator<PatternManagementRequestDto> validator, HttpContext context,
                [FromServices] IBingoRepository repository,
                CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(dto, ct);
                if (!validationResult.IsValid)
                    return ProblemResults.Validation(validationResult.ToDictionary());
                if (!context.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();
                var bingo = await repository.GetByIdAsync(id, userId, ct);
                if (bingo == null)
                    return ProblemResults.NotFound();
                bingo.AddPattern(dto.PatternId, dto.AvailablePrizes);
                await repository.UpdateAsync(bingo, ct);
                var updatedBingo = await repository.GetByIdAsync(id, userId, ct);
                return Results.Ok(updatedBingo!.ToDetailDto());
            })
            .WithSummary("Agregar patrón a bingo")
            .WithDescription("Pemrite agregar un nuevo patrón y la cantidad de premios a un bingo.")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPut("/{id:guid}/patterns", async (Guid id, [FromBody] PatternManagementRequestDto dto,
                [FromServices] IValidator<PatternManagementRequestDto> validator, HttpContext context,
                [FromServices] IBingoRepository repository,
                CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(dto, ct);
                if (!validationResult.IsValid)
                    return ProblemResults.Validation(validationResult.ToDictionary());
                if (!context.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();
                var bingo = await repository.GetByIdAsync(id, userId, ct);
                if (bingo == null)
                    return ProblemResults.NotFound();
                bingo.UpdatePatternPrizes(dto.PatternId, dto.AvailablePrizes);
                await repository.UpdateAsync(bingo, ct);
                var updatedBingo = await repository.GetByIdAsync(id, userId, ct);
                return Results.Ok(updatedBingo!.ToDetailDto());
            })
            .WithSummary("Actualizar patrón en el bingo")
            .WithDescription("Permite actualizar la cantidad de premios que hay en con un patrón")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapDelete("/{id:guid}/patterns/{patternId:guid}",
                async (Guid id, Guid patternId, [FromServices] IBingoRepository bingoRepo,
                    [FromServices] IPatternRepository patternRepo, HttpContext context, CancellationToken ct) =>
                {
                    if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();

                    var pattern = await patternRepo.GetByIdAsync(patternId, ct);
                    if (pattern is null) return ProblemResults.NotFound("El patrón especificado no existe.");

                    var bingo = await bingoRepo.GetByIdAsync(id, userId, ct);
                    if (bingo == null) return ProblemResults.NotFound();

                    bingo.RemovePattern(patternId);
                    await bingoRepo.UpdateAsync(bingo, ct);

                    return Results.Ok(bingo.ToDetailDto());
                })
            .WithSummary("Elimina un patrón de un bingo")
            .Produces<BingoSummaryDto>(200)
            .ProducesProblem(404);
    }
}