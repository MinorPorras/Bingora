using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.Common;
using Bingora.Application.DTOs.User;
using Bingora.Application.Helpers;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Common;
using Bingora.Domain.Enums;
using FluentValidation;

namespace Bingora.Api.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Gestión de usuarios")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.Admin)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/", async ([AsParameters] PaginatedQuery query, IUserRepository repository, CancellationToken ct) =>
            {
                var paged = await repository.GetPagedAsync(query.PageNumber, query.PageSize, ct);
                return Results.Ok(paged.MapTo(u => u.ToResponseDto()));
            })
            .WithSummary("Obtener todos los usuarios")
            .WithDescription(
                "Permite a un administrador obtener un listado paginado de todos los usuarios del sistema.")
            .Produces<PaginatedResult<UserResponseDto>>(201);

        group.MapGet("/{id:guid}", async (Guid id, IUserRepository repository, CancellationToken ct) =>
            {
                var user = await repository.GetByIdAsync(id, ct);
                return user is null
                    ? ProblemResults.NotFound("Usuario no encontrado.")
                    : Results.Ok(user.ToResponseDto());
            })
            .WithSummary("Obtener usuario por ID")
            .WithDescription("Obtiene el usuario con el ID especificado")
            .Produces<UserResponseDto>(200)
            .ProducesProblem(404);

        group.MapPatch("/{id:Guid}",
                async (Guid id, UpdateUsernameRequestDto dto, IValidator<UpdateUsernameRequestDto> validator,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return ProblemResults.Validation(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");

                    user.UpdateUsername(dto.NewUsername);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateUsername")
            .WithDescription("Actualiza el nombre de usuario de un usuario")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapPatch("/{id:Guid}/email",
                async (Guid id, UpdateEmailRequestDto dto, IValidator<UpdateEmailRequestDto> validator,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return ProblemResults.Validation(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");

                    var existingEmail = await repository.GetByEmailAsync(dto.NewEmail, ct);
                    //Si el email ya está usado y es de una cuenta distinta a la propia devuelve 409
                    if (existingEmail is not null && existingEmail.Id != id)
                        return ProblemResults.Conflict("El correo ya está en uso");

                    user.UpdateEmail(dto.NewEmail);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateEmail")
            .WithDescription("Fuerza un cambio de correo (gestión administrativa)")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(409)
            .ProducesProblem(404);

        group.MapPatch("/{id:Guid}/role",
                async (Guid id, UpdateUserRoleRequestDto dto, IValidator<UpdateUserRoleRequestDto> validator,
                    HttpContext httpContext,
                    IUserRepository repository, CancellationToken ct) =>
                {
                    var validation = await validator.ValidateAsync(dto, ct);
                    if (!validation.IsValid) return ProblemResults.Validation(validation.ToDictionary());

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");
                    //Si es el mismo o se quiere quitar el role 'Admin' devuelve 400
                    if (httpContext.User.TryGetUserId(out var callerId) && callerId == user.Id &&
                        dto.NewRole != Role.Admin)
                        return ProblemResults.BadRequest("No puedes quitarle el rol Admin a tu propia cuenta");

                    user.UpdateRole(dto.NewRole);
                    await repository.UpdateAsync(user, ct);
                    return Results.NoContent();
                })
            .WithSummary("UpdateUserRole")
            .WithDescription("Cambia el rol de un usuario (Solo admin)")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400);

        group.MapPost("/{id:Guid}/activate", async (Guid id, IUserRepository repository, CancellationToken ct) =>
            {
                var user = await repository.GetByIdAsync(id, ct);
                if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");

                user.Activate();
                await repository.UpdateAsync(user, ct);
                return Results.NoContent();
            })
            .WithSummary("ActivateUser")
            .WithDescription("Cambia el estado de un usuario a activo (Solo admin)")
            .Produces(204)
            .ProducesProblem(404);

        group.MapPost("/{id:Guid}/deactivate",
                async (Guid id, HttpContext httpContext, IUserRepository repository,
                    IRefreshTokenRepository refreshTokenRepository, CancellationToken ct) =>
                {
                    //Si el usuario es el mismo no permite autodesactivarse
                    if (httpContext.User.TryGetUserId(out var callerId) && callerId == id)
                        return ProblemResults.BadRequest("No puedes desactivar tu propia cuenta.");

                    var user = await repository.GetByIdAsync(id, ct);
                    if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");

                    user.Deactivate();
                    await repository.UpdateAsync(user, ct);

                    await refreshTokenRepository.RevokeAllByUserIdAsync(id, ct);
                    return Results.NoContent();
                })
            .WithSummary("DeactivateUser")
            .WithDescription(
                "Cambia el estado de un usuario a inactivo. No se puede desactivar la cuenta propia (Solo admin)")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapPost("/{id:Guid}/reset-password",
                async (Guid id, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
                    CancellationToken ct) =>
                {
                    var user = await userRepository.GetByIdAsync(id, ct);
                    if (user is null) return ProblemResults.NotFound("Usuario no encontrado.");

                    var tempPassword = PasswordGenerator.GenerateTempPassword();
                    user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(tempPassword));
                    await userRepository.UpdateAsync(user, ct);

                    await refreshTokenRepository.RevokeAllByUserIdAsync(id, ct);
                    return Results.Ok(new ChangePasswordResponseDto(tempPassword));
                })
            .WithSummary("ResetUserPassword")
            .WithDescription(
                "Genera una contraseña temporal y revoca las sesiones anteriores activas del usuario (Solo admin)")
            .Produces<ChangePasswordResponseDto>(200)
            .ProducesProblem(404);
    }
}