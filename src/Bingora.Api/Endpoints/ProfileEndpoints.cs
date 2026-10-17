using Bingora.Api.Extensions;
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
                    ? Results.Problem(title: "Usuario no encontrado", statusCode: 404)
                    : Results.Ok(user.ToResponseDto());
            })
            .WithSummary("GetMyProfile")
            .WithDescription("Obtiene el perfil del usuario autenticado");

        group.MapPatch("/me", async (
                UpdateUsernameRequestDto dto,
                IValidator<UpdateUsernameRequestDto> validator,
                HttpContext httpContext,
                IUserRepository userRepository,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(dto, ct);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                if (!httpContext.User.TryGetUserId(out var userId))
                    return Results.Unauthorized();

                var user = await userRepository.GetByIdAsync(userId, ct);
                if (user is null)
                    return Results.Problem(title: "Usuario no encontrado.", statusCode: 404);

                user.UpdateUsername(dto.NewUsername);
                await userRepository.UpdateAsync(user, ct);

                return Results.NoContent();
            })
            .WithName("UpdateMyUsername")
            .WithSummary("Actualiza mi nombre de usuario");
        
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
                    return Results.ValidationProblem(validation.ToDictionary());

                if (!httpContext.User.TryGetUserId(out var userId))
                    return Results.Unauthorized();

                var user = await userRepository.GetByIdAsync(userId, ct);
                if (user is null)
                    return Results.Problem(title: "Usuario no encontrado.", statusCode: 404);

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    return Results.Problem(
                        title: "La contraseña actual es incorrecta.",
                        statusCode: 400);

                user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(dto.NewPassword));
                await userRepository.UpdateAsync(user, ct);

                // Buena práctica: al cambiar contraseña, todas las sesiones existentes mueren.
                // El cliente deberá hacer login de nuevo.
                await refreshTokens.RevokeAllByUserIdAsync(userId, ct);

                return Results.NoContent();
            })
            .WithName("ChangeMyPassword")
            .WithSummary("Cambia mi contraseña y cierra todas mis sesiones");
    }
}