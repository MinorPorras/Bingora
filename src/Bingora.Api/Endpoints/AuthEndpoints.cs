using Bingora.Application.DTOs.Auth;
using Bingora.Application.Interfaces;
using Bingora.Application.Validators.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Bingora.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register",
                async (RegisterRequestDto dto,
                    IValidator<RegisterRequestDto> validator,
                    IAuthService authService,
                    CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid)
                        return Results.ValidationProblem(validationResult.ToDictionary());


                    var result = await authService.RegisterAsync(dto, ct);
                    return result.IsSuccess
                        ? Results.Created($"api/users/{result.Value.UserId}", result.Value)
                        : Results.BadRequest(new ProblemDetails
                        {
                            Title = "Error de registro",
                            Detail = result.Error?.Message
                        });
                }
            )
            .WithSummary("RegisterUser")
            .WithDescription("Registra un nuevo usuario administrador")
            .Produces<AuthResponseDto>(201)
            .ProducesProblem(400)
            .AllowAnonymous()
            .RequireRateLimiting("AuthPolicy");


        group.MapPost("/login",
                async (LoginRequestDto dto,
                    IValidator<LoginRequestDto> validator,
                    IAuthService authService,
                    CancellationToken ct) =>
                {
                    var validationResult = await validator.ValidateAsync(dto, ct);
                    if (!validationResult.IsValid)
                        return Results.ValidationProblem(validationResult.ToDictionary());

                    var result = await authService.LoginAsync(dto, ct);
                    return result.IsSuccess
                        ? Results.Ok(result.Value)
                        : Results.Json(new ProblemDetails
                        {
                            Title = "Credenciales inválidas",
                            Detail = result.Error?.Message,
                            Status = 401
                        }, statusCode: 401);
                })
            .WithSummary("Login")
            .WithDescription(
                "Permite que un usuario administrador inicie sesión con sus credenciales y obtiene los tokens JWT")
            .Produces<AuthResponseDto>(200)
            .ProducesProblem(401)
            .AllowAnonymous()
            .RequireRateLimiting("AuthPolicy");

        group.MapPost("/refresh",
                async (RefreshTokenRequestDto dto,
                    IAuthService service,
                    CancellationToken ct) =>
                {
                    var result = await service.RefreshTokenAsync(dto.RefreshToken, ct);
                    return result.IsSuccess
                        ? Results.Ok(result.Value)
                        : Results.Json(new ProblemDetails
                        {
                            Title = "Token de actualización inválido",
                            Detail = result.Error?.Message,
                            Status = 401
                        }, statusCode: 401);
                })
            .WithSummary("Refresh")
            .WithDescription("Permite la regeneración del refresh token del usuario y rota sus tokens")
            .Produces(200)
            .ProducesProblem(400)
            .RequireRateLimiting("GeneralPolicy");

        group.MapPost("/logout",
                async (RefreshTokenRequestDto dto, IAuthService service, CancellationToken ct) =>
                {
                    var result = await service.LogoutAsync(dto.RefreshToken, ct);
                    return result.IsSuccess
                        ? Results.Ok()
                        : Results.Json(new ProblemDetails
                        {
                            Title = "Error al cerrar sesión",
                            Detail = result.Error?.Message,
                        });
                })
            .WithSummary("Logout")
            .WithDescription("Permite que un usuario administrador pueda cerrar sesión y elimina su refresh token")
            .Produces(200)
            .ProducesProblem(400)
            .RequireRateLimiting("GeneralPolicy");
    }
}