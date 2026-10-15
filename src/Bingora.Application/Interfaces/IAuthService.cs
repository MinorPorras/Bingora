using Bingora.Application.DTOs.Auth;
using Bingora.Domain.Common;

namespace Bingora.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto,
        CancellationToken ct = default(CancellationToken));

    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto dto, CancellationToken ct = default(CancellationToken));

    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken,
        CancellationToken ct = default(CancellationToken));

    Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default(CancellationToken));
}