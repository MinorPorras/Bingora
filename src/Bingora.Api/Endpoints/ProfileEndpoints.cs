using Bingora.Api.Extensions;
using Bingora.Api.Helpers;
using Bingora.Application.DTOs.User;
using Bingora.Application.Interfaces;
using Bingora.Application.Mappings;
using Bingora.Domain.Enums;
using FluentValidation;

namespace Bingora.Api.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile")
            .WithTags("Mi perfil")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.User)))
            .RequireRateLimiting("GeneralPolicy");

        group.MapGet("/me", async (HttpContext context, IUserRepository userRepository, CancellationToken ct) =>
            {
                if (!context.User.TryGetUserId(out var userId)) return Results.Unauthorized();
                var user = await userRepository.GetByIdAsync(userId, ct);
                return user == null
                    ? ProblemResults.NotFound("Usuario no encontrado.")
                    : Results.Ok(user.ToResponseDto());
            })
            .WithSummary("GetMyProfile")
            .WithDescription("Obtiene el perfil del usuario autenticado")
            .Produces<UserResponseDto>(200)
            .ProducesProblem(404);

        group.MapPatch("/me", async (
                UpdateUsernameRequestDto dto,
                IValidator<UpdateUsernameRequestDto> validator,
                HttpContext httpContext,
                IUserRepository userRepository,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(dto, ct);
                if (!validation.IsValid)
                    return ProblemResults.Validation(validation.ToDictionary());
                // Si no se tiene un ID no se inició sesión
                if (!httpContext.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();

                var user = await userRepository.GetByIdAsync(userId, ct);
                if (user is null)
                    return ProblemResults.NotFound("Usuario no encontrado");

                user.UpdateUsername(dto.NewUsername);
                await userRepository.UpdateAsync(user, ct);

                return Results.NoContent();
            })
            .WithName("UpdateMyUsername")
            .WithSummary("Actualiza el propio nombre de usuario de la cuenta")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);

        group.MapPost("/me/change-password", async (
                ChangePasswordRequestDto dto,
                IValidator<ChangePasswordRequestDto> validator,
                HttpContext httpContext,
                IUserRepository userRepository,
                IRefreshTokenRepository refreshTokens,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(dto, ct);
                if (!validation.IsValid)
                    return ProblemResults.Validation(validation.ToDictionary());
                // Si no se tiene un ID no se inició sesión
                if (!httpContext.User.TryGetUserId(out var userId))
                    return ProblemResults.Unauthorized();

                var user = await userRepository.GetByIdAsync(userId, ct);
                if (user is null)
                    return ProblemResults.NotFound("Usuario no encontrado");
                //Se verifica que la contraseña actual que se ingresó sea la registrada
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    return ProblemResults.BadRequest("La contraseña actual es incorrecta.");
                //Actualiza en la base de datos la contraseña nueva hasheada 
                user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(dto.NewPassword));
                await userRepository.UpdateAsync(user, ct);

                // Todas las sesiones existentes mueren.
                // El cliente deberá hacer login de nuevo.
                await refreshTokens.RevokeAllByUserIdAsync(userId, ct);

                return Results.NoContent();
            })
            .WithName("ChangeMyPassword")
            .WithSummary("Cambia mi contraseña y cierra todas mis sesiones")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404);
    }
}