using System.Security.Cryptography;
using Bingora.Api.Extensions;
using Bingora.Application.DTOs.Common;
using Bingora.Application.DTOs.User;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Application.Validators.User;
using Bingora.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bingora.Api.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Gestión de usuarios")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/", async (PaginatedQuery query, IUserRepository repository, CancellationToken ct) =>
            {
                var paged = await repository.GetPagedAsync(query.PageNumber, query.PageSize, ct);
                return Results.Ok(paged.Map(u => u.ToResponseDto()));
            })
            .WithSummary("Obtener todos los usuarios")
            .WithDescription(
                "Permite a un administrador obtener un listado paginado de todos los usuarios del sistema.");

        group.MapGet("/{id:guid}", async (Guid id, IUserRepository repository, CancellationToken ct) =>
            {
                var user = await repository.GetByIdAsync(id, ct);
                return user is null
                    ? UserNotFound()
                    : Results.Ok(new { user.Id, user.Username, user.Email, user.Role, user.IsActive, user.CreatedAt });
            })
            .WithSummary("Obtener usuario por ID")
            .WithDescription("Obtiene el usuario con el ID especificado");

        group.MapPatch("/{id:Guid}",
                async (Guid id, UpdateUsernameRequestDto dto, IValidator<UpdateUsernameRequestDto> validator,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return UserNotFound();

                    user.UpdateUsername(dto.NewUsername);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateUsername")
            .WithDescription("Actualiza el nombre de usuario de un usuario");

        group.MapPatch("/{id:Guid}/email",
                async (Guid id, UpdateEmailRequestDto dto, IValidator<UpdateEmailRequestDto> validator,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return UserNotFound();

                    var existingEmail = await repository.GetByEmailAsync(dto.NewEmail, ct);
                    if (existingEmail is not null && existingEmail.Id != id)
                        return Results.Problem(title: "El correo ya está en uso", statusCode: 409);

                    user.UpdateEmail(dto.NewEmail);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateEmail")
            .WithDescription("Fuerza un cambio de correo (gestión administrativa)");

        group.MapPatch("/{id:Guid}/role",
                async (Guid id, UpdateUserRoleRequestDto dto, IValidator<UpdateUserRoleRequestDto> validator,
                    HttpContext httpContext,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return UserNotFound();

                    if (httpContext.User.TryGetUserId(out var callerId) && callerId == user.Id &&
                        dto.NewRole != Role.Admin)
                        return Results.Problem(
                            title: "No puedes quitarle el rol Admin a tu propia cuenta",
                            statusCode: 400);

                    user.UpdateRole(dto.NewRole);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateUserRole")
            .WithDescription("Cambia el rol de un usuario (Solo admin)");

        group.MapPost("/{id:Guid}/activate", async (Guid id, IUserRepository repository, CancellationToken ct) =>
        {
            var user = await repository.GetByIdAsync(id, ct);
            if (user is null) return UserNotFound();

            user.Activate();
            await repository.UpdateAsync(user, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:Guid}/deactivate",
            async (Guid id, HttpContext httpContext, IUserRepository repository,
                IRefreshTokenRepository refreshTokenRepository, CancellationToken ct) =>
            {
                if (httpContext.User.TryGetUserId(out var callerId) && callerId == id)
                    return Results.Problem(
                        title: "No puedes desactivar tu propia cuenta.",
                        statusCode: 400);

                var user = await repository.GetByIdAsync(id, ct);
                if (user is null) return UserNotFound();

                user.Deactivate();
                await repository.UpdateAsync(user, ct);

                await refreshTokenRepository.RevokeAllByUserIdAsync(id, ct);
                return Results.NoContent();
            });

        group.MapPost("/{id:Guid}/reset-password",
                async (Guid id, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
                    CancellationToken ct) =>
                {
                    var user = await userRepository.GetByIdAsync(id, ct);
                    if (user is null) return UserNotFound();

                    var tempPassword = GenerateTempPassword();
                    user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(tempPassword));
                    await userRepository.UpdateAsync(user, ct);

                    await refreshTokenRepository.RevokeAllByUserIdAsync(id, ct);
                    return Results.Ok(new { tempPassword });
                })
            .WithSummary("ResetUserPassword")
            .WithDescription("Genera una contraseña temporal y revoca las sesiones");
    }

    private static IResult UserNotFound() =>
        Results.Problem(title: "Usuario no encontrado.", statusCode: 404);

    private static string GenerateTempPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@#$%&*";
        const string all = upper + lower + digits + symbols;

        var chars = new char[16];
        chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        for (var i = 4; i < chars.Length; i++)
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];

        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }
}