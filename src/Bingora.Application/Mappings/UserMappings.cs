using Bingora.Application.DTOs.User;
using Bingora.Domain.Entities;

namespace Bingora.Application.Mappings;

public static class UserMappings
{
    public static UserResponseDto ToResponseDto(this User user) => new(user.Id, user.Username, user.Email,
        user.Role.ToString(), user.IsActive, user.CreatedAt);
}