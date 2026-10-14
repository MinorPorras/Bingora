namespace Bingora.Application.DTOs.Auth;

public record AuthResponseDto(
    Guid UserId,
    string Username,
    string Email,
    string Token,
    DateTime TokenExpiration,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);