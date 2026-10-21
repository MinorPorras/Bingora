using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Common;
using Bingora.Application.DTOs.Pattern;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace Bingora.Api.Endpoints;

public static class PatternEndpoints
{
    public static void MapPatternEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patterns").WithTags("patterns")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.User), nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/",
                async ([AsParameters] PaginatedQuery query, HttpContext context, IPatternRepository repository,
                    CancellationToken ct) =>
                {
                    var paged = await repository.GetAllPagedAsync(query.PageNumber, query.PageSize, ct);
                    return Results.Ok(paged.MapTo(u => u.ToResponseDto()));
                })
            .WithSummary("Obtener todos los patrones")
            .WithDescription(
                "Permite a los usuarios obtener un listado paginado de todos los patrones que existen en el sistema")
            .Produces<PaginatedResult<PatternResponseDto>>();

        group.MapGet("/search/{name}",
                async ([AsParameters] PaginatedQuery query, string name, HttpContext context,
                    IPatternRepository repository,
                    CancellationToken ct) =>
                {
                    var paged = await repository.GetByNamePagedAsync(query.PageNumber, query.PageSize, name, ct);
                    return Results.Ok(paged.MapTo(u => u.ToResponseDto()));
                })
            .WithSummary("Obtener patrones por nombre")
            .WithDescription("Obtiene todos los patrones, de forma paginada, que contengan el nombre buscado.")
            .Produces<PaginatedResult<PatternResponseDto>>();

        group.MapGet("/{id:guid}",
                async (Guid id, HttpContext context, IPatternRepository repository, CancellationToken ct) =>
                {
                    var pattern = await repository.GetByIdAsync(id, ct);
                    return pattern is null
                        ? ProblemResults.NotFound("Patrón no encontrado", "No se encontró un patrón con ese Id")
                        : Results.Ok(pattern.ToResponseDto());
                })
            .WithSummary("Obtiene patrón por Id")
            .WithDescription("Obtiene el patrón que contenga el ID de tipo Guid específicado.")
            .Produces<PatternResponseDto>()
            .ProducesProblem(404);

        group.MapPost("/",
                async (CreateUpdatePatternRequestDto dto, IValidator<CreateUpdatePatternRequestDto> validator,
                    IPatternRepository repository, HttpContext context, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return ProblemResults.Validation(validation.ToDictionary());
                    var entity = Pattern.Create(dto.Name, dto.GridRepresentation);
                    await repository.AddAsync(entity, ct);
                    return Results.Created($"/api/patterns/{entity.Id}", entity.ToResponseDto());
                })
            .WithSummary("Crea un nuevo patrón")
            .WithDescription("Crea un nuevo patrón con el nombre y el patrón grid específicado.")
            .Produces<PatternResponseDto>(201)
            .ProducesProblem(400);

        group.MapPut("/{id:guid}", async (
                Guid id,
                CreateUpdatePatternRequestDto dto,
                IValidator<CreateUpdatePatternRequestDto> validator,
                IPatternRepository repository,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(dto, ct);
                if (!validation.IsValid) return ProblemResults.Validation(validation.ToDictionary());

                var entity = await repository.GetByIdAsync(id, ct);
                if (entity is null) return ProblemResults.NotFound("Patrón no encontrado");

                entity.Update(dto.Name, dto.GridRepresentation);

                await repository.UpdateAsync(entity, ct);

                return Results.Ok(entity.ToResponseDto());
            })
            .WithSummary("Actualiza los datos del patrón")
            .WithDescription("Actualiza el nombre y el patrón grid de un patrón ya existente.")
            .Produces<PatternResponseDto>(200)
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapPatch("/activate/{id:guid}",
                async (Guid id, HttpContext context, IPatternRepository repository, CancellationToken ct) =>
                {
                    var pattern = await repository.GetByIdAsync(id, ct);
                    if (pattern is null) return ProblemResults.NotFound("No se encontró el patrón a activar");
                    pattern.Activate();
                    await repository.UpdateAsync(pattern, ct);
                    return Results.NoContent();
                })
            .WithSummary("Activa un patrón")
            .WithDescription("Activa un patrón que está actualmente inactivo.")
            .Produces(204)
            .ProducesProblem(404);

        group.MapPatch("/deactivate/{id:guid}",
                async (Guid id, HttpContext context, IPatternRepository repository, CancellationToken ct) =>
                {
                    var pattern = await repository.GetByIdAsync(id, ct);
                    if (pattern is null) return ProblemResults.NotFound("No se encontró el patrón a desactivar");
                    pattern.Deactivate();
                    await repository.UpdateAsync(pattern, ct);
                    return Results.NoContent();
                })
            .WithSummary("Desactiva un patrón")
            .WithDescription("Desactiva un patrón que está actualmente activo.")
            .Produces(204)
            .ProducesProblem(404);
    }
}