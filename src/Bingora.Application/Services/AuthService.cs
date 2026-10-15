using Bingora.Application.DTOs.Auth;
using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Errors;
using Microsoft.Win32.SafeHandles;

namespace Bingora.Application.Services;

public class AuthService(IUserRepository repository, IJwtTokenGenerator jwtTokenGenerator)
    : IAuthService
{
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto,
        CancellationToken ct = default(CancellationToken))
    {
        var existingUser = await repository.GetByEmailAsync(dto.Email, ct);
        if (existingUser is not null) return Result<AuthResponseDto>.Failure(UserErrors.EmailAlreadyExists);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var newUser = new User(
            username: dto.Username,
            email: dto.Email,
            passwordHash: passwordHash
        );

        await repository.AddAsync(newUser, ct);

        var (accessToken, accessTokenExpiration, refreshToken, refreshTokenExpiration) =
            jwtTokenGenerator.GenerateToken(newUser);
        newUser.UpdateRefreshToken(refreshToken, refreshTokenExpiration);
        await repository.UpdateAsync(newUser, ct);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                newUser.Id,
                newUser.Username,
                newUser.Email,
                accessToken,
                accessTokenExpiration,
                refreshToken,
                refreshTokenExpiration
            )
        );
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto dto,
        CancellationToken ct = default(CancellationToken))
    {
        var user = await repository.GetByEmailAsync(dto.Email, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials);

        var (accessToken, accessTokenExpiration, refreshToken, refreshTokenExpiration) =
            jwtTokenGenerator.GenerateToken(user);
        user.UpdateRefreshToken(refreshToken, refreshTokenExpiration);
        await repository.UpdateAsync(user, ct);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                user.Id,
                user.Username,
                user.Email,
                accessToken,
                accessTokenExpiration,
                refreshToken,
                refreshTokenExpiration
            )
        );
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        var user = await repository.GetByRefreshTokenAsync(refreshToken, ct);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration < DateTime.UtcNow)
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidOrExpiredRefreshToken);
        var (newAccessToken, newAccessExp, newRefreshToken, newRefreshExp) = jwtTokenGenerator.GenerateToken(user);
        user.UpdateRefreshToken(newRefreshToken, newRefreshExp);
        await repository.UpdateAsync(user, ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            user.Id, 
            user.Username, 
            user.Email, 
            newAccessToken, 
            newAccessExp, 
            newRefreshToken, 
            newRefreshExp));
    }

    public async Task<Result> LogoutAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        var user = await repository.GetByRefreshTokenAsync(refreshToken, ct);
        if (user is null) return Result.Success();
        
        user.RevokeRefreshToken();
        await repository.UpdateAsync(user, ct);
        return Result.Success();
    }
}