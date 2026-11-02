namespace Bingora.Application.DTOs.Auth;

public record LoginRequestDto(
    string Input,
    string Password
);