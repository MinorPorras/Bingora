using Bingora.Application.DTOs.Auth;
using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Enums;
using Bingora.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace Bingora.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<AuthService> logger)
    : IAuthService
{
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto,
        CancellationToken ct = default(CancellationToken))
    {
        var existingUser = await userRepository.GetByEmailAsync(dto.Email, ct);
        if (existingUser is not null) return Result<AuthResponseDto>.Failure(UserErrors.EmailAlreadyExists);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var newUser = new User(
            username: dto.Username,
            email: dto.Email,
            passwordHash: passwordHash,
            true,
            Role.User
        );

        await userRepository.AddAsync(newUser, ct);
        // Obtiene el nuevo par de JWT
        var (accessToken, accessTokenExpiration, refreshTokenString, refreshTokenExpiration) =
            jwtTokenGenerator.GenerateToken(newUser);
        var newRefreshToken = new RefreshToken(refreshTokenString, newUser.Id, refreshTokenExpiration);
        await refreshTokenRepository.AddAsync(newRefreshToken, ct);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                newUser.Id,
                newUser.Username,
                newUser.Email,
                accessToken,
                accessTokenExpiration,
                refreshTokenString,
                refreshTokenExpiration
            )
        );
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto dto,
        CancellationToken ct = default(CancellationToken))
    {
        var user = await userRepository.GetByEmailAsync(dto.Email, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash) || !user.IsActive)
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials);
        // Obtiene el nuevo par de JWT
        var (accessToken, accessTokenExpiration, refreshTokenString, refreshTokenExpiration) =
            jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = new RefreshToken(refreshTokenString, user.Id, refreshTokenExpiration);
        await refreshTokenRepository.AddAsync(newRefreshToken, ct);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                user.Id,
                user.Username,
                user.Email,
                accessToken,
                accessTokenExpiration,
                refreshTokenString,
                refreshTokenExpiration
            )
        );
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (storedToken is not { RevokedAt: null })
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidOrExpiredRefreshToken);
        //Si ya expiró la desactiva
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            storedToken.Revoke(DateTime.UtcNow);
            await refreshTokenRepository.UpdateAsync(storedToken, ct);
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidOrExpiredRefreshToken);
        }
        // Si el usuario está inactivo lo revoca
        if (!storedToken.User.IsActive)
        {
            storedToken.Revoke(DateTime.UtcNow);
            await refreshTokenRepository.UpdateAsync(storedToken, ct);
            return Result<AuthResponseDto>.Failure(UserErrors.InactiveUser);
        }
        // Si el token ya fue reemplazado por otro se activan medidas contra animalias
        if (storedToken.ReplacedByTokenId.HasValue)
        {
            logger.LogWarning("ANOMALÍA DETECTADA: Reuso del refresh token del usuario {userId}.", storedToken.UserId);
            var allUserTokens = await refreshTokenRepository.GetActiveByUserIdAsync(storedToken.UserId, ct);
            foreach (var userToken in allUserTokens)
            {
                userToken.Revoke(DateTime.UtcNow);
                await refreshTokenRepository.UpdateAsync(userToken, ct);
            }

            return Result<AuthResponseDto>.Failure(UserErrors.InvalidOrExpiredRefreshToken);
        }

        var user = storedToken.User;
        var (newAccessToken, accessTokenExpiration, refreshTokenString, refreshTokenExpiration) =
            jwtTokenGenerator.GenerateToken(user);
        //Se actualiza el refresh token
        var newRefreshToken = new RefreshToken(refreshTokenString, user.Id, refreshTokenExpiration);
        storedToken.ReplaceWith(newRefreshToken.Id);

        await refreshTokenRepository.UpdateAsync(storedToken, ct);
        await refreshTokenRepository.AddAsync(newRefreshToken, ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            user.Id, user.Username, user.Email, newAccessToken, accessTokenExpiration, refreshTokenString,
            refreshTokenExpiration));
    }

    public async Task<Result> LogoutAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (storedToken is null) return Result.Success();
        // Revoca el refresh token del usuario
        storedToken.Revoke(DateTime.UtcNow);
        await refreshTokenRepository.UpdateAsync(storedToken, ct);

        return Result.Success();
    }
}