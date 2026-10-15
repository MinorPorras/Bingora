using Bingora.Application.DTOs.Auth;
using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Domain.Errors;
using Microsoft.Win32.SafeHandles;

namespace Bingora.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository repository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto,
        CancellationToken ct = default(CancellationToken))
    {
        var existingUser = await _repository.GetByEmailAsync(dto.Email, ct);
        if (existingUser is not null) return Result<AuthResponseDto>.Failure(UserErrors.EmailAlreadyExists);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var newUser = new User(
            username: dto.Username,
            email: dto.Email,
            passwordHash: passwordHash
        );

        await _repository.AddAsync(newUser, ct);

        var (accessToken, accessTokenExpiration, refreshToken, refreshTokenExpiration) =
            _jwtTokenGenerator.GenerateToken(newUser);

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
        var user = await _repository.GetByEmailAsync(dto.Email, ct);
        if (user is null || BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials);
        
        var (accessToken, accessTokenExpiration, refreshToken, refreshTokenExpiration) =
            _jwtTokenGenerator.GenerateToken(user);
        
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

    public Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthResponseDto>> LogoutAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}