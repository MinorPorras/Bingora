namespace Bingora.Application.DTOs.User;

public record UserResponseDto(Guid Id, string Username, string Email, string Role, bool IsActive, DateTime CreatedAt);