namespace Bingora.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Username,
    string Email,
    string Password
);