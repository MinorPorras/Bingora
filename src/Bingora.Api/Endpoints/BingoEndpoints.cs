using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Bingo;
using Bingora.Application.DTOs.Common;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Application.Validators.Bingo;
using Bingora.Domain.Entities;
using Bingora.Domain.Enums;
using Bingora.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bingora.Api.Endpoints;

public static class BingoEndpoints
{
    public static void MapBingoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bingos").WithTags("Bingo")
            .RequireAuthorization(p => p.RequireRole(nameof(Role.User)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/",
            async ([AsParameters] PaginatedQuery query, HttpContext context, IBingoRepository repository,
                CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                var paged = await repository.GetPagedAsync(userId, query.PageNumber, query.PageSize, ct);
                return Results.Ok(paged.MapTo(b => b.ToResponseDto()));
            });

        group.MapGet("/search/{name}",
            async (string name, [AsParameters] PaginatedQuery query, HttpContext context, IBingoRepository repository,
                CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                var paged = await repository.GetPagedByNameAsync(userId, name, query.PageNumber, query.PageSize, ct);
                return Results.Ok(paged.MapTo(b => b.ToResponseDto()));
            });

        group.MapGet("/{id:guid}",
            async (Guid id, HttpContext context, IBingoRepository repository, CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId)) return ProblemResults.Unauthorized();
                var result = await repository.GetByIdAsync(id, userId, ct);
                return result is null
                    ? ProblemResults.NotFound()
                    : Results.Ok(result.ToResponseDto());
            });

        group.MapPost("/",
            async (CreateOrUpdateBingoRequestDto dto, CreateOrUpdateBingoRequestValidator validator,
                HttpContext context, IBingoRepository repository, CancellationToken ct) =>
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
                return Results.Created($"/api/bingos/{bingo.Id}", bingo.ToResponseDto());
            });

        group.MapPut("/{id:guid}",
            async (Guid id, CreateOrUpdateBingoRequestDto dto, CreateOrUpdateBingoRequestValidator validator,
                HttpContext context,
                IBingoRepository repository, CancellationToken ct) =>
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
                return Results.Ok(bingo.ToResponseDto());
            });

        group.MapPost("/{id:guid}/activate",
            async (Guid id, HttpContext context, IBingoRepository repository, CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();
                var bingo = await repository.GetByIdAsync(id, userId, ct);
                if (bingo == null)
                    return ProblemResults.NotFound();
                bingo.Activate();
                await repository.UpdateAsync(bingo, ct);
                return Results.Ok();
            });

        group.MapPost("/{id:guid}/deactivate",
            async (Guid id, HttpContext context, IBingoRepository repository, CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();
                var bingo = await repository.GetByIdAsync(id, userId, ct);
                if (bingo == null)
                    return ProblemResults.NotFound();
                bingo.Deactivate();
                await repository.UpdateAsync(bingo, ct);
                return Results.Ok();
            });

        group.MapPost("/{id:guid}/patterns", async (Guid id, PatternManagementRequestDto dto,
            PatternManagementRequestValidator validator, HttpContext context, IBingoRepository repository,
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
            return Results.Ok(bingo.ToResponseDto());
        });

        group.MapPut("/{id:guid}/patterns", async (Guid id, PatternManagementRequestDto dto,
            PatternManagementRequestValidator validator, HttpContext context, IBingoRepository repository,
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
            return Results.Ok(bingo.ToResponseDto());
        });

        group.MapDelete("/{id:guid}/patterns", async (Guid id, PatternManagementRequestDto dto,
            PatternManagementRequestValidator validator, HttpContext context, IBingoRepository repository,
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
            bingo.RemovePattern(dto.PatternId);
            await repository.UpdateAsync(bingo, ct);
            return Results.NoContent();
        });
    }
}